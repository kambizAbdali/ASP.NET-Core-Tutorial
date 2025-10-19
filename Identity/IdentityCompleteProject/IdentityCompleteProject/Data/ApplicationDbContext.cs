using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IdentityCompleteProject.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityCompleteProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Custom DbSets
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure composite key for IdentityUserLogin
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
            });

            // Custom configurations for User entity
            builder.Entity<User>(entity =>
            {
                // Index for National Code (must be unique)
                entity.HasIndex(u => u.NationalCode).IsUnique();

                // Required field configurations
                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.NationalCode)
                    .IsRequired()
                    .HasMaxLength(10);

                // Configure relationships
                entity.HasMany(u => u.Addresses)
                    .WithOne(ua => ua.User)
                    .HasForeignKey(ua => ua.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Purchases)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure UserAddress entity
            builder.Entity<UserAddress>(entity =>
            {
                entity.HasIndex(ua => new { ua.UserId, ua.IsPrimary })
                    .HasFilter("[IsPrimary] = 1");
            });

            // Configure Role entity
            builder.Entity<Role>(entity =>
            {
                entity.HasMany(r => r.Permissions)
                    .WithOne(rp => rp.Role)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed initial data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed initial roles
            var adminRole = new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Administrator with full access"
            };

            var managerRole = new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Manager",
                NormalizedName = "MANAGER",
                Description = "Manager with limited administrative access"
            };

            var userRole = new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "User",
                NormalizedName = "USER",
                Description = "Regular user"
            };

            builder.Entity<Role>().HasData(adminRole, managerRole, userRole);
        }
    }
}