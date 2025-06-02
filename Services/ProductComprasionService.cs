using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HindApp.Models;

namespace HindApp.Services
{
    public class ProductComparisonService
    {
        private readonly SQLiteAsyncConnection _db;

        public ProductComparisonService(SQLiteAsyncConnection db)
        {
            _db = db;
        }

        public async Task<List<ProductPriceInfo>> GetProductPricesAsync(int productId, bool ascending)
        {
            var prices = await _db.QueryAsync<ProductPriceWithStore>(
                @"SELECT ProductPrices.Id, ProductPrices.Price, ProductPrices.LastUpdated, Stores.Name AS StoreName
                  FROM ProductPrices
                  JOIN Stores ON Stores.Id = ProductPrices.StoreId
                  WHERE ProductPrices.ProductId = ?",
                  productId
            );
            
            return (ascending
                ? prices.OrderBy(p => p.Price)
                : prices.OrderByDescending(p => p.Price))
                .Select(p => new ProductPriceInfo
                {
                    Id = p.Id,
                    StoreName = p.StoreName,
                    Price = p.Price,
                    LastUpdated = p.LastUpdated
                })
                .ToList();
        }

        private class ProductPriceWithStore
        {
            public int Id { get; set; }
            public double Price { get; set; }
            public DateTime LastUpdated { get; set; }
            public string StoreName { get; set; }
        }
    }

}
