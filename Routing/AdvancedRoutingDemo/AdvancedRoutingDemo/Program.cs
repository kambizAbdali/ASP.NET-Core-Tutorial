using AdvancedRoutingDemo.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddNewtonsoftJson();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("RoutingDemo"));

// Add API controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Advanced Routing API",
        Version = "v1",
        Description = "A comprehensive demo of ASP.NET Core Routing"
    });

    // Include XML comments for Swagger - با شرط وجود فایل
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Advanced Routing API v1");
        c.RoutePrefix = "api-docs";
    });

    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Conventional Routing for MVC
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Multiple custom conventional routes
app.MapControllerRoute(
    name: "products",
    pattern: "products/{action=List}/{category?}",
    defaults: new { controller = "Product" });

app.MapControllerRoute(
    name: "blog",
    pattern: "blog/{year:int}/{month:int}/{title}",
    defaults: new { controller = "Blog", action = "Details" });

app.MapControllerRoute(
    name: "seo-friendly",
    pattern: "catalog/{category}/{productName}",
    defaults: new { controller = "Product", action = "SeoDetails" });

// Default route (should be last)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Catch-all route for 404 errors
app.MapControllerRoute(
    name: "404",
    pattern: "{*url}",
    defaults: new { controller = "Home", action = "NotFound" });

// Map API controllers
app.MapControllers();

// Seed initial data
SeedData(app);

app.Run();

void SeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Add sample products
    context.Products.AddRange(
        new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 999.99m, Description = "High-performance laptop" },
        new Product { Id = 2, Name = "Book", Category = "Education", Price = 29.99m, Description = "Programming book" },
        new Product { Id = 3, Name = "Phone", Category = "Electronics", Price = 499.99m, Description = "Smartphone" },
        new Product { Id = 4, Name = "Tablet", Category = "Electronics", Price = 299.99m, Description = "Android tablet" }
    );

    // Add sample users
    context.Users.AddRange(
        new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
        new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com" },
        new User { Id = 3, Name = "Bob Johnson", Email = "bob@example.com" }
    );

    // Add sample blog posts
    context.BlogPosts.AddRange(
        new BlogPost { Id = 1, Title = "Introduction to ASP.NET Core Routing", Content = "Routing is a fundamental concept...", PublishedDate = new DateTime(2024, 1, 15), Author = "John Doe" },
        new BlogPost { Id = 2, Title = "Advanced Routing Techniques", Content = "Learn about attribute routing...", PublishedDate = new DateTime(2024, 2, 1), Author = "Jane Smith" },
        new BlogPost { Id = 3, Title = "SEO-Friendly URLs in ASP.NET Core", Content = "Creating SEO-friendly URLs...", PublishedDate = new DateTime(2024, 2, 10), Author = "Bob Johnson" }
    );

    context.SaveChanges();
}