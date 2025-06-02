using SQLite;
using System;

namespace HindApp.Models
{
    [Table("Users")]
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

    [Table("Category")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }
    }

    [Table("Products")]
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? CategoryId { get; set; }
    }

    [Table("Stores")] 
    public class Store
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Name { get; set; }

        public string Location { get; set; }
    }

    [Table("ProductPrices")]
    public class ProductPrice
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int ProductId { get; set; }

        [NotNull]
        public int StoreId { get; set; }

        [NotNull]
        public double Price { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    [Table("Favorites")]
    public class Favorite
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int UserId { get; set; }

        [NotNull]
        public int ProductPricesId  { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("Barcodes")]
    public class Barcode
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int ProductId { get; set; }

        [Unique, NotNull]
        public string BarcodeValue { get; set; }
    }

    [Table("ProductImages")]
    public class ProductImage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int ProductId { get; set; }

        [NotNull]
        public string ImageUrl { get; set; }

        public string Caption { get; set; }
    }

    [Table("ProductRatings")]
    public class ProductRating
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int ProductId { get; set; }

        [NotNull]
        public int UserId { get; set; }

        [NotNull]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("Inventory")]
    public class Inventory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int StoreId { get; set; }

        [NotNull]
        public int ProductId { get; set; }

        public int Quantity { get; set; } = 0;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    [Table("PriceHistory")]
    public class PriceHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int ProductId { get; set; }

        [NotNull]
        public int StoreId { get; set; }

        [NotNull]
        public double OldPrice { get; set; }

        [NotNull]
        public double NewPrice { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
