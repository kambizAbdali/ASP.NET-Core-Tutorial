using Microsoft.EntityFrameworkCore;
using TagHelpersDemo.Models;

namespace TagHelpersDemo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public void SeedData()
        {
            if (!Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Laptops", Type = CategoryType.Electronics },
                    new Category { Id = 2, Name = "Smartphones", Type = CategoryType.Electronics },
                    new Category { Id = 3, Name = "T-Shirts", Type = CategoryType.Clothing },
                    new Category { Id = 4, Name = "Programming Books", Type = CategoryType.Books }
                };
                Categories.AddRange(categories);
                SaveChanges();
            }

            if (!Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { Id = 1, Name = "Gaming Laptop", Price = 1299.99m, CategoryId = 1, Status = ProductStatus.Available, InStock = true, StockQuantity = 15 },
                    new Product { Id = 2, Name = "Smartphone X", Price = 799.99m, CategoryId = 2, Status = ProductStatus.Available, InStock = true, StockQuantity = 25 },
                    new Product { Id = 3, Name = "Cotton T-Shirt", Price = 19.99m, CategoryId = 3, Status = ProductStatus.OutOfStock, InStock = false, StockQuantity = 0 },
                    new Product { Id = 4, Name = "C# Programming Guide", Price = 49.99m, CategoryId = 4, Status = ProductStatus.ComingSoon, InStock = false, StockQuantity = 100 }
                };
                Products.AddRange(products);
                SaveChanges();
            }
        }
    }
}