using SQLite;
using HindApp.Models;
using System.Diagnostics;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _connection;

    public DatabaseService(string dbPath)
    {
        _connection = new SQLiteAsyncConnection(dbPath);
        EnableForeignKeys();
    }

    public async Task InitializeAsync()
    {
        var result = await _connection.ExecuteScalarAsync<int>(
            "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='Users';");

        if (result == 0)
        {
            // Создание всех таблиц вручную
            string createScript = @"
            -- 1. Пользователи
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    IsAdmin INTEGER NOT NULL DEFAULT 0 -- 1 = админ, 0 = обычный пользователь
);

CREATE TABLE Category(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
    );

-- 2. Продукты
CREATE TABLE Products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    CategoryId INTEGER,
    FOREIGN KEY (CategoryId) REFERENCES Category(Id)
);

-- 3. Магазины
CREATE TABLE Stores (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Location TEXT -- можно расширить до широта/долгота или адреса
);

-- 4. Цены продуктов в магазинах
CREATE TABLE ProductPrices (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL,
    StoreId INTEGER NOT NULL,
    Price REAL NOT NULL,
    LastUpdated DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (StoreId) REFERENCES Stores(Id)
);

-- 5. Избранные товары пользователей
CREATE TABLE Favorites (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    AddedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    UNIQUE(UserId, ProductId) -- чтобы один пользователь не добавил один товар дважды
);

-- 6. Штрихкоды (опционально)
CREATE TABLE Barcodes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL,
    Barcode TEXT NOT NULL UNIQUE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- 7. Изображения продуктов
CREATE TABLE ProductImages (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL,
    ImageUrl TEXT NOT NULL,
    Caption TEXT,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- 8. Отзывы и оценки
CREATE TABLE ProductRatings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    Rating INTEGER CHECK(Rating >= 1 AND Rating <= 5),
    Comment TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- 9. Наличие товара (инвентарь)
CREATE TABLE Inventory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    StoreId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    Quantity INTEGER DEFAULT 0,
    LastUpdated DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (StoreId) REFERENCES Stores(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    UNIQUE(StoreId, ProductId)
);

-- 10. История изменения цен
CREATE TABLE PriceHistory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL,
    StoreId INTEGER NOT NULL,
    OldPrice REAL NOT NULL,
    NewPrice REAL NOT NULL,
    ChangedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (StoreId) REFERENCES Stores(Id)
);
"
            ;

            var statements = createScript.Split(';')
                           .Select(s => s.Trim())
                           .Where(s => !string.IsNullOrWhiteSpace(s));

            foreach (var stmt in statements)
            {
                await _connection.ExecuteAsync(stmt + ";");
            }
        }

        await SeedAsync(); // Добавляем вызов
    }

    private async void EnableForeignKeys()
    {
        await _connection.ExecuteAsync("PRAGMA foreign_keys = ON;");
    }

    private async Task SeedAsync()
    {
        // Проверим, есть ли уже магазины
        var existingStores = await _connection.Table<Store>().ToListAsync();
        if (existingStores.Any()) return; // Уже есть данные — выходим

        // Магазины
        var stores = new List<Store>
        {
            new Store { Name = "Магнит", Location = "Москва, ул. Ленина, 10" },
            new Store { Name = "Пятёрочка", Location = "Санкт-Петербург, Невский проспект, 24" },
            new Store { Name = "Лента", Location = "Новосибирск, ул. Фрунзе, 51" },
            new Store { Name = "О’Кей", Location = "Екатеринбург, ул. Мира, 12" },
            new Store { Name = "Ашан", Location = "Казань, проспект Победы, 98" }
        };

        var categories = new List<Category>
        {
            new Category { Name = "Молочные продукты" },
            new Category { Name = "Хлеб и выпечка" },
            new Category { Name = "Бакалея" },
            new Category { Name = "Яйца и молочные продукты" },
            new Category { Name = "Масла и жиры" },
            new Category { Name = "Мясо и птица" },
            new Category { Name = "Овощи и фрукты" },
            new Category { Name = "Сладости" },
        };

        // Продукты
        var products = new List<Product>
        {
            new Product { Name = "Молоко 3.2% 1л", Description = "Пастеризованное молоко в пакете", CategoryId = 1 },
            new Product { Name = "Хлеб пшеничный", Description = "Свежий хлеб из пшеничной муки", CategoryId = 2 },
            new Product { Name = "Яйца куриные 10 шт", Description = "Яйца первой категории", CategoryId = 4 },
            new Product { Name = "Макароны рожки 500г", Description = "Макароны из твердых сортов пшеницы", CategoryId = 3 },
            new Product { Name = "Сахар 1 кг", Description = "Сахар-песок фасованный", CategoryId = 3 },
            new Product { Name = "Масло подсолнечное 1л", Description = "Рафинированное масло без запаха", CategoryId = 5 },
            new Product { Name = "Куриное филе 1 кг", Description = "Охлаждённое куриное филе", CategoryId = 6 },
            new Product { Name = "Картофель 1 кг", Description = "Свежий картофель, урожай 2024 года", CategoryId = 7 },
            new Product { Name = "Яблоки красные 1 кг", Description = "Яблоки сорта \"Гала\"", CategoryId = 7 },
            new Product { Name = "Шоколад молочный 90г", Description = "Классический молочный шоколад", CategoryId = 8 }
        };

        var productPrices = new List<ProductPrice>
        {
            new ProductPrice { LastUpdated = new DateTime(2025, 6, 1), Price = 1.52, ProductId = 1, StoreId = 1 },
            new ProductPrice { LastUpdated = new DateTime(2025, 5, 25), Price = 1.71, ProductId = 1, StoreId = 2 },
            new ProductPrice { LastUpdated = new DateTime(2025, 5, 27), Price = 1.46, ProductId = 1, StoreId = 3 },
            new ProductPrice { LastUpdated = new DateTime(2025, 5, 15), Price = 2.17, ProductId = 1, StoreId = 4 },
            new ProductPrice { LastUpdated = new DateTime(2025, 6, 1), Price = 1.52, ProductId = 2, StoreId = 1 },
            new ProductPrice { LastUpdated = new DateTime(2025, 5, 25), Price = 1.71, ProductId = 3, StoreId = 2 },
            new ProductPrice { LastUpdated = new DateTime(2025, 5, 27), Price = 1.46, ProductId = 2, StoreId = 3 },
            new ProductPrice { LastUpdated = new DateTime(2025, 5, 15), Price = 2.17, ProductId = 3, StoreId = 4 },
            new ProductPrice { LastUpdated = new DateTime(2025, 6, 1), Price = 1.52, ProductId = 5, StoreId = 1 },
            new ProductPrice { LastUpdated = new DateTime(2025, 5, 25), Price = 1.71, ProductId = 4, StoreId = 2 },
            new ProductPrice { LastUpdated = new DateTime(2025, 5, 27), Price = 1.46, ProductId = 4, StoreId = 3 },
            new ProductPrice { LastUpdated = new DateTime(2025, 5, 15), Price = 2.17, ProductId = 5, StoreId = 4 },
            new ProductPrice { LastUpdated = new DateTime(2025, 5, 15), Price = 2.17, ProductId = 6, StoreId = 5 },
        };

        // Сохраняем в базу
        try
        {
            await _connection.InsertAllAsync(stores);
            await _connection.InsertAllAsync(categories);
            await _connection.InsertAllAsync(products);
            await _connection.InsertAllAsync(productPrices);
        }
        catch(SQLite.SQLiteException e)
        {
            Debug.WriteLine(e);
        }
        
    }
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _connection.Table<Product>().ToListAsync();
    }

    public SQLiteAsyncConnection GetConnection() => _connection;
}
