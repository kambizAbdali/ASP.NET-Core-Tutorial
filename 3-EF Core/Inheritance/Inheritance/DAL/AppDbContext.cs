using Inheritance.Models;
using Microsoft.EntityFrameworkCore;

namespace Inheritance.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CreditCardPayment> CreditCardPayments { get; set; } // فقط برای TPT و TPC
        public DbSet<BankTransferPayment> BankTransferPayments { get; set; } // فقط برای TPT و TPC

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // **Settings for each inheritance strategy:**

            // *** 1. Table-per-Hierarchy (TPH) ***
            // (This strategy is the default and does not require special configuration, but you can customize the discriminator column)
            //modelBuilder.Entity<Payment>()
            //    .HasDiscriminator<string>("PaymentType")
            //    .HasValue<CreditCardPayment>("CreditCardPayments")
            //    .HasValue<BankTransferPayment>("BankTransferPayments")
            //    .HasValue<Payment>("Payments"); // If you want Payment to exist. Otherwise, it is optional.
            //base.OnModelCreating(modelBuilder);

            // *** 2. Table-per-Type (TPT) ***
            //modelBuilder.Entity<Payment>().ToTable("Payments");
            //modelBuilder.Entity<CreditCardPayment>().ToTable("CreditCardPayments");
            //modelBuilder.Entity<BankTransferPayment>().ToTable("BankTransferPayments");

            ///* *** 3. Table-per-Concrete-Class (TPC) - (manual configuration required) ****/
            // TPC is not directly supported in EF Core. You must map manually.
            // This code is an approach (not fully complete) to simulate TPC:
            modelBuilder.Entity<CreditCardPayment>().ToTable("CreditCardPayments");
            modelBuilder.Entity<BankTransferPayment>().ToTable("BankTransferPayments");

            // **Very important**: With TPC, inheritance is not fully managed by EF Core. You must manage the logic for reading/writing data to the derived tables yourself. For example, EF Core does not automatically generate joined queries.}
        }
    }
}