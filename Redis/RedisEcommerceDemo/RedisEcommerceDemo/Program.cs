using RedisEcommerceDemo.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (string.IsNullOrEmpty(redisConnectionString))
{
    throw new InvalidOperationException("Redis connection string is not configured");
}

// Register Redis Connection Multiplexer as singleton (recommended by StackExchange.Redis)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.AbortOnConnectFail = false; // Continue even if Redis is not available
    configuration.ConnectTimeout = 5000; // 5 seconds connection timeout
    
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Connecting to Redis at: {RedisConnection}", redisConnectionString);
    
    return ConnectionMultiplexer.Connect(configuration);
});

// Register Redis Distributed Cache for IDistributedCache interface
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = builder.Configuration["RedisSettings:InstanceName"] ?? "Ecommerce_";
});

// Register application services
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

// Add health checks (optional but recommended)
builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString, name: "redis");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map health check endpoint
app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Global exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An unhandled exception occurred");
        throw;
    }
});

app.Run();