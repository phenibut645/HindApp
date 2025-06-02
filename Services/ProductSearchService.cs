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

        public async Task<List<Product>> SearchProductsAsync(string query, int limit, int? categoryId = null)
        {
            query = query?.Trim().ToLower();

            var allProducts = await _db.Table<Product>().ToListAsync();

            var filtered = allProducts
                .Where(p => (!categoryId.HasValue || p.CategoryId == categoryId.Value));

            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = filtered
                    .Where(p => p.Name.ToLower().Contains(query))
                    .OrderBy(p => p.Name.ToLower().IndexOf(query))
                    .ThenBy(p => p.Name.Length);
            }

            return filtered.Take(limit).ToList();
        }



    }

}