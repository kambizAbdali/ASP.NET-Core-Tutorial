using LoadingRelationships.Application.Services;
using LoadingRelationships.Core.Entities;
using LoadingRelationships.Core.Interfaces;
using LoadingRelationships.Infrastructure.Data;
using LoadingRelationships.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

//Enable  Lazy Loading
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if (!context.Users.Any())
        {
            var user = new User
            {
                Username = "Kami",
                Email = "KambizAbdali@Gmail.com",
                Home = new Address { Street = "123 Main St", City = "Kermanshah", PostalCode = "12345", Country = "Iran" },
                WorkPlace = new Address { Street = "456 Office Rd", City = "Tehran", PostalCode = "67890", Country = "Iran" }
            };
            context.Users.Add(user);
            context.Users.ToList();
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();