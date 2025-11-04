using LoggingDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoggingDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public HomeController(
            ILogger<HomeController> logger,
            IUserService userService,
            IProductService productService)
        {
            _logger = logger;
            _userService = userService;
            _productService = productService;

            // Log controller initialization
            _logger.LogInformation("HomeController initialized");
        }

        public IActionResult Index()
        {
            _logger.LogDebug("Home/Index action requested");

            // Simulate some business logic
            ViewBag.UserCount = _userService.GetAllUsers().Count;
            ViewBag.ProductCount = _productService.GetProducts().Count;

            _logger.LogInformation("Home page loaded successfully");
            return View();
        }

        public IActionResult Privacy()
        {
            _logger.LogDebug("Home/Privacy action requested");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError("Error page displayed - likely an unhandled exception occurred");
            return View();
        }

        // Action to demonstrate different log levels
        public IActionResult TestLogging()
        {
            _logger.LogTrace("This is a TRACE message - very detailed information");
            _logger.LogDebug("This is a DEBUG message - useful for debugging");
            _logger.LogInformation("This is an INFORMATION message - normal operation");
            _logger.LogWarning("This is a WARNING message - something unexpected but handled");
            _logger.LogError("This is an ERROR message - something failed");
            _logger.LogCritical("This is a CRITICAL message - system stability at risk");

            TempData["Message"] = "All log levels tested successfully!";
            return RedirectToAction("Index");
        }

        // Action to simulate an exception
        public IActionResult SimulateError()
        {
            try
            {
                _logger.LogWarning("About to simulate an exception...");
                throw new InvalidOperationException("This is a simulated error for logging demonstration");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in SimulateError action");
                TempData["Error"] = "Error simulated and logged successfully!";
                return RedirectToAction("Index");
            }
        }

        // Action to simulate critical error
        public IActionResult SimulateCritical()
        {
            _productService.SimulateCriticalError();
            TempData["Message"] = "Critical error scenario simulated!";
            return RedirectToAction("Index");
        }
    }
}