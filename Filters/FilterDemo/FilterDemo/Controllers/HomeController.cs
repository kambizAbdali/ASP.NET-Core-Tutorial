using Microsoft.AspNetCore.Mvc;
using FilterDemo.Models;
using FilterDemo.Filters.ActionFilter;
using FilterDemo.Filters.AuthorizationFilter;
using FilterDemo.Filters.ExceptionFilter;
using FilterDemo.Filters.ResourceFilter;
using FilterDemo.Filters.ResultFilter;

namespace FilterDemo.Controllers
{
    // Apply global filters to all actions
    // اعمال فیلترهای سراسری به تمام اکشن‌ها
    [AddHeaderResultFilter] // Result Filter
    [ServiceFilter(typeof(LogActionFilter))] // Action Filter
    public class HomeController : Controller
    {
        private static List<User> _users = new()
        {
            new User { Id = 1, Name = "John Doe", Email = "john@example.com", Role = "Admin" },
            new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Role = "User" }
        };

        private static List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 10 },
            new Product { Id = 2, Name = "Mouse", Price = 29.99m, Stock = 50 }
        };

        public IActionResult Index()
        {
            ViewBag.Message = "Welcome to Filter Demo Application";
            return View();
        }

        // API Key Authorization example
        // مثال احراز هویت با کلید API
        [HttpGet]
        [RequireApiKey] // Authorization Filter
        [Route("api/users")]
        public IActionResult GetUsers()
        {
            Console.WriteLine("📋 Returning users list");
            return Ok(_users);
        }

        // Role-based Authorization example
        // مثال احراز هویت مبتنی بر نقش
        [HttpGet]
        [RequireRole("Admin")] // Authorization Filter
        [Route("api/admin/users")]
        public IActionResult GetAdminUsers()
        {
            Console.WriteLine("👨‍💼 Returning admin users");
            return Ok(_users.Where(u => u.Role == "Admin"));
        }

        // Cache example
        // مثال کش
        [HttpGet]
        [TypeFilter(typeof(CacheResourceFilter))] // Resource Filter
        [Route("api/products")]
        public IActionResult GetProducts()
        {
            // Simulate processing time
            // شبیه‌سازی زمان پردازش
            Thread.Sleep(500);
            Console.WriteLine("📦 Returning products list");
            return Ok(_products);
        }

        // Model validation example
        // مثال اعتبارسنجی مدل
        [HttpPost]
        [ValidateModel] // Action Filter
        [Route("api/users")]
        public IActionResult CreateUser([FromBody] User user)
        {
            user.Id = _users.Max(u => u.Id) + 1;
            _users.Add(user);

            Console.WriteLine($"✅ User created: {user.Name}");
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        // Exception example
        // مثال خطا
        [HttpGet]
        [Route("api/error/test")]
        public IActionResult TestError()
        {
            // This will trigger the exception filter
            // این خطا باعث فعال شدن فیلتر خطا می‌شود
            throw new InvalidOperationException("This is a test exception from the controller");
        }

        // Business exception example
        // مثال خطای کسب‌وکار
        [HttpGet]
        [Route("api/business/error")]
        public IActionResult TestBusinessError()
        {
            throw new InvalidOperationException("Business rule violation: Product out of stock");
        }

        // Format response example
        // مثال فرمت پاسخ
        [HttpGet]
        [TypeFilter(typeof(FormatResponseFilter))] // Result Filter
        [Route("api/formatted/products")]
        public IActionResult GetFormattedProducts()
        {
            return Ok(_products);
        }

        // Timing example with simulated delay
        // مثال زمان‌سنجی با تاخیر شبیه‌سازی شده
        [HttpGet]
        [TypeFilter(typeof(TimingResourceFilter))] // Resource Filter
        [Route("api/slow/products")]
        public IActionResult GetSlowProducts()
        {
            // Warning: Long execution time simulation
            // هشدار: شبیه‌سازی زمان اجرای طولانی
            Thread.Sleep(1500); // 1.5 seconds delay
            Console.WriteLine("🐢 Slow products endpoint completed");
            return Ok(_products);
        }
    }
}