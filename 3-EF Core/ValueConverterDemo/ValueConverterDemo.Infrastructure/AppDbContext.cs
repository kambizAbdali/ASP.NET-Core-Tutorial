using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sequences.Infrastructure.ValueConverters;
using ValueConverterDemo.Core.Models;
using ValueConverterDemo.Infrastructure.ValueConverters;

namespace ValueConverterDemo.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()  //Example #1
                .Property(p => p.PreferredShipping)
                .HasConversion(new ShippingPreferenceConverter());

            modelBuilder.Entity<Product>().Property(p => p.Active).HasConversion(new BoolToStrConverter()); //Example #2


            modelBuilder.Entity<Note>()  //Example #3
               .Property(n => n.SecretNote)
               .HasConversion(new Base64StringConverter());


            modelBuilder.Entity<Note>().HasData(new Note {Id=1 ,SecretNote = "This is kami" }); // Id is requiered in data seeding even id was a key

            base.OnModelCreating(modelBuilder);
        }
    }
}