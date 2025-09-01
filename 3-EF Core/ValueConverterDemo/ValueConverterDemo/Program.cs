
using Microsoft.EntityFrameworkCore;
using ValueConverterDemo.Core.Models;
using ValueConverterDemo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        //context.Database.Migrate(); // Apply any pending migrations
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Seed Data
        if (!context.Products.Any()) // Check if there's any data already
        {
            context.Products.AddRange(
                new Product { ProductName = "Laptop", Price = 1200, PreferredShipping = ShippingPreference.Express, Active = false },
                new Product { ProductName = "Mouse", Price = 25, PreferredShipping = ShippingPreference.Standard }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
