using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Services;
using TaskManagement.Infrastructure.Seed;
using TaskManagement.Core.Interfaces.IServices;
using TaskManagement.Core.Interfaces.IRepositories;

namespace TaskManagement.Infrastructure
{
    /// <summary>
    /// Dependency injection setup for Infrastructure layer
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Entity Framework with In-Memory database
            services.AddDbContext<TaskManagementContext>(options =>
                options.UseInMemoryDatabase("TaskManagementDb"));

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserTokenRepository, UserTokenRepository>();

            // Register services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            // Register seeder
            services.AddScoped<DatabaseSeeder>();

            return services;
        }
    }
}