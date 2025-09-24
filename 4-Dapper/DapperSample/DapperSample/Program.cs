using Dapper;
using DapperSample.Application.Services;
using DapperSample.Core.Interfaces;
using DapperSample.Infrastructure.Data;
using DapperSample.Infrastructure.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // For IHostEnvironment
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

// Build the application
var builder = WebApplication.CreateBuilder(args);

// ===== 1) Configuration =====
// Get the connection string for the main database
var configuration = builder.Configuration.GetConnectionString("DefaultConnection");

// ===== 2) Dependency Injection registrations =====
builder.Services.AddSingleton(new DbConnectionFactory(configuration));

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Services
builder.Services.AddScoped<ProductService, ProductService>();
builder.Services.AddScoped<CategoryService, CategoryService>();
builder.Services.AddScoped<CustomerService, CustomerService>();

// Fluent Migrator
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(configuration)
        .ScanIn(typeof(DapperSample.Infrastructure.Migrations.InitialCreate).Assembly)
        .For.Migrations())
    .AddScoped<FluentMigrator.Runner.Processors.ProcessorOptions>(sp =>
    {
        return new FluentMigrator.Runner.Processors.ProcessorOptions
        {
            Timeout = TimeSpan.FromSeconds(60), // Customize as needed
            PreviewOnly = false,
            ProviderSwitches = string.Empty
        };
    });

builder.Services.AddControllersWithViews();

// ===== 3) Build the app =====
var app = builder.Build();

// ===== 4) Configure the HTTP request pipeline =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

// ===== 5) Database initialization & migrations =====
// Run DB creation and migrations as part of startup
await InitializeDatabaseAsync(app);

// Run the app
app.Run();

// ===== Local methods =====
static async Task InitializeDatabaseAsync(IApplicationBuilder app)
{
    // Create a DI scope to resolve services
    using var scope = app.ApplicationServices.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        // Retrieve the main connection string
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        var mainConnectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(mainConnectionString))
        {
            throw new InvalidOperationException("DefaultConnection is not configured.");
        }

        // 1) Extract the database name from the connection string
        var builderCS = new SqlConnectionStringBuilder(mainConnectionString);
        string databaseName = builderCS.InitialCatalog;

        // 2) Build a connection string to master without the target database
        builderCS.InitialCatalog = "master";
        string masterConnectionString = builderCS.ConnectionString;

        // 3) Create the database if it doesn't exist using Dapper
        using (var connection = new SqlConnection(masterConnectionString))
        {
            await connection.OpenAsync();

            // SQL to create database if it doesn't exist
            string createDbQuery = $@"
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = @dbName)
BEGIN
    EXEC ('CREATE DATABASE ' + QUOTENAME(@dbName));
END;";

            await connection.ExecuteAsync(createDbQuery, new { dbName = databaseName });
        }

        // 4) Run FluentMigrator migrations
        var migrationRunner = services.GetRequiredService<IMigrationRunner>();
        migrationRunner.MigrateUp();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating or migrating the database.");
        // Depending on your needs, you might rethrow or swallow the exception
        // throw;
    }
}
