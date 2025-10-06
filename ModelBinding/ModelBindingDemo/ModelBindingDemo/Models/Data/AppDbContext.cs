using Microsoft.EntityFrameworkCore;
using ModelBindingDemo.Models.Entities;

namespace ModelBindingDemo.Models.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Person>()
                .HasMany(p => p.PhoneNumbers)
                .WithOne(pn => pn.Person)
                .HasForeignKey(pn => pn.PersonId);

            modelBuilder.Entity<Person>()
                .HasOne(p => p.Address)
                .WithOne(a => a.Person)
                .HasForeignKey<Address>(a => a.PersonId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            // Seed initial data with Description
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Price = 999.99m,
                    Category = "Electronics",
                    Description = "High-performance laptop",
                    InStock = true,
                    CreatedAt = DateTime.Now.AddDays(-10)
                },
                new Product
                {
                    Id = 2,
                    Name = "Mouse",
                    Price = 29.99m,
                    Category = "Electronics",
                    Description = "Wireless mouse",
                    InStock = true,
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Product
                {
                    Id = 3,
                    Name = "Keyboard",
                    Price = 49.99m,
                    Category = "Electronics",
                    Description = "Mechanical keyboard",
                    InStock = false,
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new Product
                {
                    Id = 4,
                    Name = "Monitor",
                    Price = 199.99m,
                    Category = "Electronics",
                    Description = "27-inch monitor",
                    InStock = true,
                    CreatedAt = DateTime.Now.AddDays(-1)
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@example.com",
                    CreatedAt = DateTime.Now.AddMonths(-1),
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Username = "john",
                    Email = "john@example.com",
                    CreatedAt = DateTime.Now.AddDays(-15),
                    IsActive = true
                }
            );
        }
    }
}