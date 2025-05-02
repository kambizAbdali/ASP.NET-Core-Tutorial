using GeneratedValues.Model;
using Microsoft.EntityFrameworkCore;

namespace GeneratedValues.DAL
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                // Id is an identity column (default behavior)  
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Name and Custom Value  
                entity.Property(e => e.CustomValue)
                    .ValueGeneratedNever();

                // Remove Identity from IdentityCol  
                entity.Property(e => e.IdentityColumn).ValueGeneratedNever();

                // Default Value  
                entity.Property(e => e.CreatedAdd)
                    .HasDefaultValueSql("GETDATE()");

                // Computed Column  
                entity.Property(e => e.ComputedCol)
                    .HasComputedColumnSql("'B' + FORMAT(GETDATE(), 'yyyy-MM-dd') + '-' + CAST(OrderNumber AS NVARCHAR(10))");
            });
        }
    }
}