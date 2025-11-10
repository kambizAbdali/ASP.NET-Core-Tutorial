using RedisEcommerceDemo.Models;
using RedisEcommerceDemo.Services;

namespace RedisEcommerceDemo.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> _mockProducts;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ILogger<ProductRepository> logger)
        {
            _logger = logger;

            // Mock data - in real application this would come from database
            _mockProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop Gaming Pro", Description = "High-performance gaming laptop", Price = 1500, StockQuantity = 10, Tags = new List<string> { "electronics", "gaming" } },
            new Product { Id = 2, Name = "Wireless Headphones", Description = "Noise-cancelling wireless headphones", Price = 200, StockQuantity = 25, Tags = new List<string> { "electronics", "audio" } },
            new Product { Id = 3, Name = "Smartphone X", Description = "Latest smartphone with advanced features", Price = 800, StockQuantity = 15, Tags = new List<string> { "electronics", "mobile" } },
            new Product { Id = 4, Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard for gamers", Price = 120, StockQuantity = 30, Tags = new List<string> { "electronics", "gaming" } },
            new Product { Id = 5, Name = "Monitor 4K", Description = "27-inch 4K UHD monitor", Price = 400, StockQuantity = 8, Tags = new List<string> { "electronics", "display" } }
        };
        }

        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            _logger.LogInformation("Fetching featured products from database");
            await Task.Delay(100); // Simulate database latency
            return _mockProducts.Take(3).ToList();
        }

        public async Task<List<Product>> GetBestSellingProductsAsync()
        {
            _logger.LogInformation("Fetching best-selling products from database");
            await Task.Delay(150); // Simulate database latency
            return _mockProducts.OrderBy(p => p.Id).Take(3).ToList();
        }

        public async Task<List<Product>> GetNewArrivalsAsync()
        {
            _logger.LogInformation("Fetching new arrivals from database");
            await Task.Delay(120); // Simulate database latency
            return _mockProducts.OrderByDescending(p => p.CreatedDate).Take(3).ToList();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Fetching product {ProductId} from database", id);
            await Task.Delay(80); // Simulate database latency
            return _mockProducts.FirstOrDefault(p => p.Id == id);
        }

        public async Task<bool> UpdateProductViewCountAsync(int productId)
        {
            var product = _mockProducts.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                product.ViewCount++;
                _logger.LogInformation("Updated view count for product {ProductId}: {ViewCount}", productId, product.ViewCount);
                return true;
            }
            return false;
        }

        public async Task<HomePageViewModel> GetHomePageDataAsync()
        {
            _logger.LogInformation("Fetching home page data from database");

            // Simulate complex database query with multiple joins
            await Task.Delay(200);

            return new HomePageViewModel
            {
                FeaturedProducts = await GetFeaturedProductsAsync(),
                BestSellingProducts = await GetBestSellingProductsAsync(),
                NewArrivals = await GetNewArrivalsAsync(),
                CategoryStats = new Dictionary<string, int>
            {
                { "electronics", 15 },
                { "gaming", 8 },
                { "audio", 5 }
            },
                CacheTimestamp = DateTime.UtcNow,
                DataSource = "database"
            };
        }
    }
}