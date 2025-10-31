using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Infrastructure.Data
{
    /// <summary>
    /// DbContext for the Task Management application using In-Memory database
    /// </summary>
    public class TaskManagementContext : DbContext
    {
        public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure TaskItem entity
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Description).HasMaxLength(500);
                entity.Property(t => t.Priority).HasMaxLength(10);
                entity.Property(t => t.CreatedDate).IsRequired();

                // Configure relationship with User
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tasks)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(100);
                entity.Property(u => u.CreatedAt).IsRequired();
            });

            // Configure UserToken entity
            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.HasKey(ut => ut.Id);
                entity.Property(ut => ut.RefreshToken).IsRequired();
                entity.Property(ut => ut.RefreshTokenExpiry).IsRequired();
                entity.Property(ut => ut.CreatedAt).IsRequired();

                // Configure relationship with User
                entity.HasOne(ut => ut.User)
                      .WithMany(u => u.UserTokens)
                      .HasForeignKey(ut => ut.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}