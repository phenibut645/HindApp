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

        /// <summary>
        /// Получить цены на продукт во всех магазинах с сортировкой по цене.
        /// </summary>
        /// <param name="productId">ID продукта</param>
        /// <param name="ascending">true — сортировка от дешевых к дорогим, false — наоборот</param>
        /// <returns>Список цен продукта по магазинам</returns>
        public async Task<List<ProductPriceInfo>> GetProductPricesAsync(int productId, bool ascending)
        {
            var prices = await _db.QueryAsync<ProductPriceWithStore>(
                @"SELECT ProductPrices.Price, ProductPrices.LastUpdated, Stores.Name AS StoreName
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
                    StoreName = p.StoreName,
                    Price = p.Price,
                    LastUpdated = p.LastUpdated
                })
                .ToList();
        }

        private class ProductPriceWithStore
        {
            public double Price { get; set; }
            public DateTime LastUpdated { get; set; }
            public string StoreName { get; set; }
        }
    }

}
