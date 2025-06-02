using HindApp.Models;
using SQLite;
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
    ProductPricesId INTEGER NOT NULL,
    AddedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ProductPricesId) REFERENCES ProductPrices(Id),
    UNIQUE(UserId, ProductPricesId) -- чтобы один пользователь не добавил один товар дважды
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

        await SeedAsync();
    }



    private async void EnableForeignKeys()
    {
        await _connection.ExecuteAsync("PRAGMA foreign_keys = ON;");
    }


    private async Task SeedAsync()
    {
        var existingStores = await _connection.Table<Store>().ToListAsync();
        if (existingStores.Any()) return;

        var stores = new List<Store>
        {
            new Store { Name = "Rimi", Location = "Tallinn, Estonia" },
            new Store { Name = "Selver", Location = "Tallinn, Estonia" },
            new Store { Name = "Prisma", Location = "Helsinki, Finland" },
            new Store { Name = "Maxima", Location = "Vilnius, Lithuania" },
            new Store { Name = "Solaris", Location = "Tallinn, Estonia" },
        };

        var categories = new List<Category>
        {
            new Category { Name = "Piimatooted" },
            new Category { Name = "Leib ja sai" },
            new Category { Name = "Kuivained" },
            new Category { Name = "Munad ja piimatooted" },
            new Category { Name = "Õlid ja rasvad" },
            new Category { Name = "Liha ja linnuliha" },
            new Category { Name = "Köögiviljad ja puuviljad" },
            new Category { Name = "Maiustused" },
        };

        var products = new List<Product>
        {
            new Product { Name = "Piim 3.2% 1l", Description = "Pastöriseeritud piim pakendis", CategoryId = 1 },
            new Product { Name = "Nisuleib", Description = "Värske nisujahust leib", CategoryId = 2 },
            new Product { Name = "Kanamunad 10 tk", Description = "Esimese kategooria munad", CategoryId = 4 },
            new Product { Name = "Makaronid torukesed 500g", Description = "Kõrgekvaliteedilisest nisust makaronid", CategoryId = 3 },
            new Product { Name = "Suhkur 1 kg", Description = "Pakendatud suhkur", CategoryId = 3 },
            new Product { Name = "Päevalilleõli 1l", Description = "Lõhnatu rafineeritud õli", CategoryId = 5 },
            new Product { Name = "Kanafilee 1 kg", Description = "Jahutatud kanafilee", CategoryId = 6 },
            new Product { Name = "Kartul 1 kg", Description = "Värske kartul, 2024. aasta saak", CategoryId = 7 },
            new Product { Name = "Punased õunad 1 kg", Description = "\"Gala\" sort õunad", CategoryId = 7 },
            new Product { Name = "Piimašokolaad 90g", Description = "Klassikaline piimašokolaad", CategoryId = 8 }
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

        var users = new List<User>
        {
            new User { IsAdmin = 1, PasswordHash = "space", Username = "root" },
            new User { IsAdmin = 0, PasswordHash = "space", Username = "user" }
        };

        try
        {
            await _connection.InsertAllAsync(stores);
            await _connection.InsertAllAsync(categories);
            await _connection.InsertAllAsync(products);
            await _connection.InsertAllAsync(productPrices);
            await _connection.InsertAllAsync(users);
        }
        catch(SQLite.SQLiteException e)
        {
            Debug.WriteLine(e);
        }
        
    }

    public Task<List<Category>> GetAllCategoriesAsync()
    {
        return _connection.Table<Category>().ToListAsync();
    }


    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _connection.Table<Product>().ToListAsync();
    }

    public SQLiteAsyncConnection GetConnection() => _connection;

    public async Task<User?> GetUserByUsernameAndPasswordAsync(string username, string password)
    {
        return await _connection.Table<User>()
            .Where(u => u.Username == username && u.PasswordHash == password)
            .FirstOrDefaultAsync();
    }

    public Task<int> AddProductAsync(Product product)
    {
        return _connection.InsertAsync(product);
    }

    public Task<int> UpdateProductAsync(Product product)
    {
        return _connection.UpdateAsync(product);
    }

    public Task<int> DeleteProductAsync(Product product)
    {
        return _connection.DeleteAsync(product);
    }

    public async Task<List<Store>> GetAllStoresAsync()
    {
        return await _connection.Table<Store>().ToListAsync();
    }
    public Task<int> AddStoreAsync(Store store)
    {
        return _connection.InsertAsync(store);
    }

    public Task<int> UpdateStoreAsync(Store store)
    {
        return _connection.UpdateAsync(store);
    }

    public Task<int> DeleteStoreAsync(Store store)
    {
        return _connection.DeleteAsync(store);
    }
    public async Task<List<ProductPrice>> GetStoreProductPricesAsync(int storeId)
    {
        var connection = GetConnection();
        return await connection.Table<ProductPrice>()
            .Where(pp => pp.StoreId == storeId)
            .ToListAsync();
    }

    public async Task AddOrUpdateProductPriceAsync(ProductPrice productPrice)
    {
        var connection = GetConnection();

        var existing = await connection.Table<ProductPrice>()
            .Where(pp => pp.ProductId == productPrice.ProductId && pp.StoreId == productPrice.StoreId)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            existing.Price = productPrice.Price;
            existing.LastUpdated = DateTime.Now;
            await connection.UpdateAsync(existing);
        }
        else
        {
            productPrice.LastUpdated = DateTime.Now;
            await connection.InsertAsync(productPrice);
        }
    }

    public async Task DeleteProductPriceAsync(int productPriceId)
    {
        var connection = GetConnection();
        var pp = await connection.FindAsync<ProductPrice>(productPriceId);
        if (pp != null)
        {
            await connection.DeleteAsync(pp);
        }
    }


   
    public Task AddCategoryAsync(Category category)
    {
        return _connection.InsertAsync(category);
    }

    
    public Task UpdateCategoryAsync(Category category)
    {
        return _connection.UpdateAsync(category);
    }

    
    public Task DeleteCategoryAsync(int categoryId)
    {
        return _connection.DeleteAsync<Category>(categoryId);
    }


}
