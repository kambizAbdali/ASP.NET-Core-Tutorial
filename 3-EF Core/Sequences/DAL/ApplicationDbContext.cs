// Sequences.Data/ApplicationDbContext.cs
using Sequences.Models;
using Microsoft.EntityFrameworkCore;

namespace Sequences.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure HiLo sequence
            modelBuilder.HasSequence<int>("ProductHiLoSequence")
                        .StartsAt(1)
                        .IncrementsBy(10);

            modelBuilder.Entity<Product>()
                .Property(o => o.Id)
                .HasDefaultValueSql("NEXT VALUE FOR ProductHiLoSequence"); // Use the sequence name without "dbo."

            base.OnModelCreating(modelBuilder);
        }
    }
}
