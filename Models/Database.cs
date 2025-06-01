using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace HindApp.Models
{


    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull]
        public string Username { get; set; }

        [NotNull]
        public string PasswordHash { get; set; }

        [NotNull]
        public int IsAdmin { get; set; } = 0;
    }

    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }
    }

    public class Store
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }

        public string Location { get; set; }
    }

    public class ProductPrice
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int ProductId { get; set; }

        [Indexed, NotNull]
        public int StoreId { get; set; }

        [NotNull]
        public double Price { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class Favorite
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int UserId { get; set; }

        [Indexed, NotNull]
        public int ProductId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }

    public class Barcode
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int ProductId { get; set; }

        [Unique, NotNull]
        public string BarcodeValue { get; set; }
    }

    public class ProductImage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int ProductId { get; set; }

        [NotNull]
        public string ImageUrl { get; set; }

        public string Caption { get; set; }
    }

    public class ProductRating
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int ProductId { get; set; }

        [Indexed, NotNull]
        public int UserId { get; set; }

        public int Rating { get; set; } // 1–5

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Inventory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int StoreId { get; set; }

        [Indexed, NotNull]
        public int ProductId { get; set; }

        public int Quantity { get; set; } = 0;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class PriceHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public int ProductId { get; set; }

        [Indexed, NotNull]
        public int StoreId { get; set; }

        [NotNull]
        public double OldPrice { get; set; }

        [NotNull]
        public double NewPrice { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
