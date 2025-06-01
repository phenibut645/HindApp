using SQLite;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using HindApp.Models;

namespace HindApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitializeAsync()
        {
            await _db.CreateTableAsync<User>();
            await _db.CreateTableAsync<Product>();
            await _db.CreateTableAsync<Store>();
            await _db.CreateTableAsync<ProductPrice>();
            await _db.CreateTableAsync<Favorite>();
            await _db.CreateTableAsync<Barcode>();
            await _db.CreateTableAsync<ProductImage>();
            await _db.CreateTableAsync<ProductRating>();
            await _db.CreateTableAsync<Inventory>();
            await _db.CreateTableAsync<PriceHistory>();
        }

        public SQLiteAsyncConnection GetConnection() => _db;
    }
}