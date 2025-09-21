using LoadingRelationships.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingRelationships.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Lazy Loading:  Requires installing Microsoft.EntityFrameworkCore.Proxies
            // Enable lazy loading
            //   this.ChangeTracker.LazyLoadingEnabled = true; // Enabled by default


            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking; // Global NoTracking 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().OwnsOne(o => o.WorkPlace);
            modelBuilder.Entity<User>().OwnsOne(o => o.Home);


            // حذف منطقی به صورت سراسری
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsRemoved);

            // Seed Data
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Price = 1500, IsRemoved = false },
                new Product { Id = 2, Name = "Keyboard", Price = 100, IsRemoved = false }
            );

            // Seed User
            //modelBuilder.Entity<User>().HasData(
            //   new User
            //   {
            //       UserId = 1,
            //       Username = "Kami",
            //       Email = "KambizAbdali@Gmail.com",
            //       Home = new Address { Street = "123 Main St", City = "Kermanshah", PostalCode = "12345", Country = "Iran" },
            //       WorkPlace = new Address { Street = "456 Office Rd", City = "Tehran", PostalCode = "67890", Country = "Iran" }
            //   }
            //);
        }
    }
}
