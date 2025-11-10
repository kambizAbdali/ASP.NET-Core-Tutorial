# Redis E-Commerce Demo - Complete Implementation Guide

## 📋 Project Overview
A complete ASP.NET Core MVC 6.0 e-commerce application demonstrating Redis integration for caching, session management, and real-time data operations.

## 🏗️ Project Structure
```
RedisEcommerceDemo/
├── Controllers/
│   ├── HomeController.cs
│   ├── CartController.cs
│   └── HealthController.cs
├── Models/
│   ├── Product.cs
│   ├── ShoppingCart.cs
│   ├── CartItem.cs
│   └── HomePageViewModel.cs
├── Services/
│   ├── IRedisService.cs
│   ├── RedisService.cs
│   ├── IShoppingCartService.cs
│   ├── ShoppingCartService.cs
│   ├── IProductRepository.cs
│   └── ProductRepository.cs
├── Views/
│   ├── Home/
│   ├── Cart/
│   └── Shared/
└── Program.cs
```

## 🚀 Core Features Implemented

### 1. Redis Data Types Implementation
- **Strings**: Counters, cache storage, simple key-value pairs
- **Hashes**: User profiles, object storage with multiple fields
- **Lists**: Activity logs, message queues, recent items
- **Sets**: Unique tags, categories, membership checks
- **Sorted Sets**: Leaderboards, rankings, scored collections

### 2. Shopping Cart System
- Complete cart management with Redis persistence
- Real-time cache operations visibility
- Cart item quantity management
- Checkout process with cache validation
- Cart health monitoring

### 3. Caching Strategies
- GetOrSet pattern with fallback mechanisms
- Sliding and absolute expiration policies
- Cache invalidation strategies
- Graceful degradation when Redis is unavailable

### 4. Real-time Monitoring
- Live cache operation logging
- Performance metrics display
- Connection health checks
- Visual cache status indicators

## 🛠️ Technical Implementation

### Configuration (Program.cs)
```csharp
// Redis Configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = builder.Configuration["RedisSettings:InstanceName"] ?? "Ecommerce_";
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString, name: "redis")
    .AddUrlGroup(new Uri("https://localhost:7001"), name: "base_url");
```

### Key Services

#### RedisService
- Complete Redis operations for all data types
- JSON serialization for complex objects
- Error handling and fallback mechanisms
- Connection management

#### ShoppingCartService
- Cart persistence using Redis JSON storage
- Item management with quantity control
- Cart merging and synchronization
- Health monitoring and diagnostics

### Models

#### Product
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int ViewCount { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
}
```

#### ShoppingCart
```csharp
public class ShoppingCart
{
    public int UserId { get; set; }
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
```

## 🎯 Implemented Controllers

### HomeController
- Product catalog display with Redis caching
- Home page data aggregation
- Redis demo page with all data type examples
- Cache management operations

### CartController
- Complete cart CRUD operations
- Checkout process with cache validation
- Real-time cart updates
- Health monitoring endpoints
- Debug and diagnostic endpoints

### HealthController
- Redis connection health checks
- Performance metrics API
- System status monitoring

## 📊 Redis Key Patterns
```
cart:user:{userId}          # Shopping cart storage
product:detail:{productId}   # Product cache
homepage:data               # Home page cache
demo:counter               # Demo counters
demo:activities            # Activity lists
demo:tags                  # Tag sets
demo:leaderboard           # Sorted set rankings
```

## 🔧 Setup and Configuration

### 1. Prerequisites
- .NET 6.0 SDK
- Redis Server (local or remote)
- IDE (Visual Studio 2022+ or VS Code)

### 2. Installation Steps
1. Clone the repository
2. Update `appsettings.json` with Redis connection string
3. Run Redis server on localhost:6379
4. Build and run the application

### 3. Configuration File
```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379,abortConnect=false"
  },
  "RedisSettings": {
    "InstanceName": "Ecommerce_",
    "DefaultCacheDuration": 30
  }
}
```

## 🎮 Usage Guide

### 1. Starting the Application
- Run the application and navigate to `/Home/Index`
- Observe Redis cache operations in real-time
- Test shopping cart functionality

### 2. Testing Cart Operations
1. Browse products on home page
2. Add items to cart (observe Redis operations)
3. View cart to see persisted data
4. Test quantity updates and removals
5. Complete checkout process


## 📈 Performance Features

### Cache Optimization
- Lazy loading with GetOrSet pattern
- Appropriate TTL configurations
- Batch operations for multiple items
- Connection pooling and multiplexing

### Error Handling
- Graceful fallback when Redis is unavailable
- Comprehensive logging
- User-friendly error messages
- Automatic recovery mechanisms

### Monitoring
- Real-time operation logging
- Performance metrics display
- Health check endpoints
- Connection status monitoring

## 🔍 Diagnostic Endpoints

### Health Checks
- `GET /health` - Overall system health
- `GET /api/health/redis` - Redis-specific health
- `GET /api/cart/health` - Cart service health

### Debug Endpoints
- `GET /debug/cart/{userId}` - Cart data inspection
- `POST /debug/cart/reset/{userId}` - Cart reset
- `GET /api/cart/summary` - Cart summary API

## 🛡️ Security Features

- Anti-forgery token validation
- Input validation and model binding
- Secure Redis configuration
- Error message sanitization

## 📝 Key Implementation Notes

### Redis Serialization
- Complex objects use JSON serialization
- Simple objects use hash field storage
- Custom serialization for ShoppingCart objects
- Proper type conversion handling

### Cache Patterns
- Read-Through caching with GetOrSet
- Write-Through for cart updates
- Cache-Aside for product data
- Time-based expiration strategies

### Error Recovery
- Automatic cart recreation on corruption
- Fallback to in-memory storage
- Comprehensive exception handling
- User notification system

This implementation provides a complete, production-ready e-commerce cart system with comprehensive Redis integration, real-time monitoring, and robust error handling.