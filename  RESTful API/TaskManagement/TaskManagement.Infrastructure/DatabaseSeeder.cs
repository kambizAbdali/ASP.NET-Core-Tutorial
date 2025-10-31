using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Interfaces.IServices;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Seed
{
    /// <summary>
    /// Database seeder for initial data population
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly TaskManagementContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public DatabaseSeeder(TaskManagementContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Seed users if none exist
            if (!_context.Users.Any())
            {
                await SeedUsersAsync();
                await SeedTasksAsync();
            }
        }

        private async Task SeedUsersAsync()
        {
            var users = new[]
            {
                new User
                {
                    Username = "admin",
                    PasswordHash = _passwordHasher.HashPassword("admin123"),
                    Email = "admin@taskmanagement.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "user1",
                    PasswordHash = _passwordHasher.HashPassword("user123"),
                    Email = "user1@taskmanagement.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }

        private async Task SeedTasksAsync()
        {
            var admin = await _context.Users.FirstAsync(u => u.Username == "admin");
            var user1 = await _context.Users.FirstAsync(u => u.Username == "user1");

            var tasks = new[]
            {
                new TaskItem
                {
                    Title = "Complete API Documentation",
                    Description = "Write comprehensive documentation for all RESTful endpoints",
                    Priority = "High",
                    IsCompleted = false,
                    UserId = admin.Id,
                    CreatedDate = DateTime.UtcNow.AddDays(-2)
                },
                new TaskItem
                {
                    Title = "Implement HATEOAS",
                    Description = "Add HATEOAS links to all API responses",
                    Priority = "Medium",
                    IsCompleted = true,
                    UserId = admin.Id,
                    CreatedDate = DateTime.UtcNow.AddDays(-5)
                },
                new TaskItem
                {
                    Title = "Setup JWT Authentication",
                    Description = "Implement JWT token generation and validation",
                    Priority = "High",
                    IsCompleted = false,
                    UserId = user1.Id,
                    CreatedDate = DateTime.UtcNow.AddDays(-1)
                },
                new TaskItem
                {
                    Title = "Add API Versioning",
                    Description = "Implement API versioning using URL path versioning",
                    Priority = "Low",
                    IsCompleted = false,
                    UserId = user1.Id,
                    CreatedDate = DateTime.UtcNow
                }
            };

            await _context.Tasks.AddRangeAsync(tasks);
            await _context.SaveChangesAsync();
        }
    }
}