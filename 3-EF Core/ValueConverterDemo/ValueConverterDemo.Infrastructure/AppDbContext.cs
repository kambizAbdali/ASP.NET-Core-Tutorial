using Microsoft.EntityFrameworkCore;
using Sequences.Infrastructure.ValueConverters;
using ValueConverterDemo.Core.Models;

namespace ValueConverterDemo.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.PreferredShipping)
                .HasConversion(new ShippingPreferenceConverter());

            base.OnModelCreating(modelBuilder);
        }
    }
}