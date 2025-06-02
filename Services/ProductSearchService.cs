using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HindApp.Models;
using System.Diagnostics;

namespace HindApp.Services
{
    public class ProductSearchService
    {
        private readonly SQLiteAsyncConnection _db;

        public ProductSearchService(SQLiteAsyncConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Поиск продуктов по части названия с сортировкой по степени совпадения.
        /// </summary>
        /// <param name="query">Поисковый запрос (часть названия)</param>
        /// <param name="limit">Максимальное количество возвращаемых результатов</param>
        /// <returns>Список найденных продуктов, отсортированных по релевантности</returns>
        public async Task<List<Product>> SearchProductsAsync(string query, int limit)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Product>();

            query = query.Trim().ToLower();

            // Грузим все продукты — в реальности можно ограничить (если их очень много)
            var allProducts = await _db.Table<Product>().ToListAsync();

            var matches = allProducts
                .Where(p => p.Name.ToLower().Contains(query))
                .OrderBy(p =>
                {
                    var index = p.Name.ToLower().IndexOf(query);
                    return index >= 0 ? index : int.MaxValue;
                })
                .ThenBy(p => p.Name.Length)
                .Take(limit)
                .ToList();

            return matches;
        }

    }

}