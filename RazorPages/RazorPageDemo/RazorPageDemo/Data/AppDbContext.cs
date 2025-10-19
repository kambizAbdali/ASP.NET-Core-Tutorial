using Microsoft.EntityFrameworkCore;
using RazorPageDemo.Models;

namespace RazorPageDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // داده‌های اولیه
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics", Description = "High-performance laptop", CreatedDate = DateTime.Now.AddDays(-10) },
                new Product { Id = 2, Name = "Mouse", Price = 25.50m, Category = "Electronics", Description = "Wireless mouse", CreatedDate = DateTime.Now.AddDays(-5) },
                new Product { Id = 3, Name = "Book", Price = 15.99m, Category = "Education", Description = "Programming book", CreatedDate = DateTime.Now.AddDays(-2) }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}