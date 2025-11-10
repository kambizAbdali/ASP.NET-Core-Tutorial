using Microsoft.AspNetCore.Mvc;
using RedisEcommerceDemo.Models;
using RedisEcommerceDemo.Services;

namespace RedisEcommerceDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IRedisService _redisService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IProductRepository productRepository,
            IRedisService redisService,
            ILogger<HomeController> logger)
        {
            _productRepository = productRepository;
            _redisService = redisService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Define cache key for home page data
            string cacheKey = "homepage:data";
            HomePageViewModel model;
            string dataSource = "cache";

            try
            {
                // Use GetOrSet pattern: try cache first, fallback to database
                model = await _redisService.GetOrSetAsync(
                    key: cacheKey,
                    factory: async () =>
                    {
                        dataSource = "database";
                        return await _productRepository.GetHomePageDataAsync();
                    },
                    expiry: TimeSpan.FromMinutes(5) // Cache for 5 minutes
                );

                // Update data source information
                model.DataSource = dataSource;

                _logger.LogInformation("Home page data loaded from {DataSource}", dataSource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page data");

                // Fallback: load directly from database if Redis fails
                model = await _productRepository.GetHomePageDataAsync();
                model.DataSource = "database (fallback)";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ClearHomePageCache()
        {
            // Clear home page cache (admin functionality)
            string cacheKey = "homepage:data";
            bool success = await _redisService.DeleteKeyAsync(cacheKey);

            if (success)
            {
                TempData["Message"] = "Home page cache cleared successfully!";
                _logger.LogInformation("Home page cache cleared by user");
            }
            else
            {
                TempData["Error"] = "Failed to clear cache";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Product(int id)
        {
            // Cache key for individual product
            string cacheKey = $"product:detail:{id}";
            Product? product;
            string dataSource = "cache";

            try
            {
                // Try to get product from cache
                product = await _redisService.GetOrSetAsync(
                    key: cacheKey,
                    factory: async () =>
                    {
                        dataSource = "database";
                        var freshProduct = await _productRepository.GetProductByIdAsync(id);

                        if (freshProduct != null)
                        {
                            // Update view count when product is fetched from database
                            await _productRepository.UpdateProductViewCountAsync(id);
                        }

                        return freshProduct;
                    },
                    expiry: TimeSpan.FromMinutes(10) // Cache product details for 10 minutes
                );

                if (product == null)
                {
                    return NotFound();
                }

                ViewBag.DataSource = dataSource;

                _logger.LogInformation("Product {ProductId} loaded from {DataSource}", id, dataSource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product {ProductId}", id);

                // Fallback to database
                product = await _productRepository.GetProductByIdAsync(id);
                ViewBag.DataSource = "database (fallback)";

                if (product == null)
                {
                    return NotFound();
                }
            }

            return View(product);
        }

        public async Task<IActionResult> RedisDemo()
        {
            // Demonstration page showing all Redis data types in action
            var demoModel = new RedisDemoViewModel();

            // STRING DEMO - Counter
            demoModel.StringDemo = await _redisService.IncrementAsync("demo:counter");

            // LIST DEMO - Recent activities
            await _redisService.ListLeftPushAsync("demo:activities", $"Page visited at {DateTime.UtcNow:HH:mm:ss}");
            demoModel.ListDemo = await _redisService.ListRangeAsync<string>("demo:activities", 0, 4);

            // SET DEMO - Unique tags
            await _redisService.SetAddAsync("demo:tags", "redis");
            await _redisService.SetAddAsync("demo:tags", "dotnet");
            await _redisService.SetAddAsync("demo:tags", "mvc");
            demoModel.SetDemo = await _redisService.SetMembersAsync<string>("demo:tags");

            // SORTED SET DEMO - Leaderboard
            await _redisService.SortedSetAddAsync("demo:leaderboard", "Player1", 100);
            await _redisService.SortedSetAddAsync("demo:leaderboard", "Player2", 150);
            await _redisService.SortedSetAddAsync("demo:leaderboard", "Player3", 75);
            demoModel.SortedSetDemo = await _redisService.SortedSetRangeByRankAsync("demo:leaderboard", 0, -1, true);

            // HASH DEMO - User profile
            var userProfile = new { Name = "Demo User", Email = "demo@example.com", Level = "Premium" };
            await _redisService.SetHashAsync("demo:user:profile", userProfile);
            demoModel.HashDemo = await _redisService.GetHashAsync<dynamic>("demo:user:profile");

            return View(demoModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }

    // ViewModel for Redis demo page
    public class RedisDemoViewModel
    {
        public long StringDemo { get; set; }
        public List<string> ListDemo { get; set; } = new List<string>();
        public HashSet<string> SetDemo { get; set; } = new HashSet<string>();
        public List<KeyValuePair<string, double>> SortedSetDemo { get; set; } = new List<KeyValuePair<string, double>>();
        public dynamic? HashDemo { get; set; }
    }
}