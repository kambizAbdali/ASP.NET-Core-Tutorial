using AdvancedRoutingDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Docs.Samples;

namespace AdvancedRoutingDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Conventional Routing: /Home/Index or /
        public IActionResult Index()
        {
            ViewBag.RouteInfo = ControllerContext.ToCtxString();
            return View();
        }

        // Conventional Routing: /Home/About
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            ViewBag.RouteInfo = ControllerContext.ToCtxString();
            return View();
        }

        // Route with constraint: /Home/User/5
        [HttpGet("Home/User/{id:int}")]
        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.RouteInfo = ControllerContext.ToCtxString(id, $"User ID: {id}");
            return View(user);
        }

        // Multiple parameters example
        [Route("search/{category}/{keyword}")]
        public IActionResult Search(string category, string keyword, int page = 1)
        {
            ViewBag.RouteInfo = ControllerContext.ToCtxString(msg: $"Category: {category}, Keyword: {keyword}, Page: {page}");

            ViewBag.Category = category;
            ViewBag.Keyword = keyword;
            ViewBag.Page = page;

            return View();
        }

        // Multiple routes for same action
        [Route("privacy")]
        [Route("policy")]
        [Route("Home/Privacy")]
        public IActionResult Privacy()
        {
            ViewBag.RouteInfo = ControllerContext.ToCtxString();
            return View();
        }

        // Catch-all route handler
        [Route("error/404")]
        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            ViewBag.RouteInfo = ControllerContext.ToCtxString();
            return View();
        }

        // Demonstration of URL generation
        public IActionResult UrlGenerationDemo()
        {
            var urls = new Dictionary<string, string>
            {
                ["Action"] = Url.Action("Index", "Home"),
                ["RouteUrl"] = Url.RouteUrl("default", new { controller = "Product", action = "List" }),
                ["Action with parameters"] = Url.Action("Details", "Product", new { id = 5, name = "laptop" }),
                ["Absolute URL"] = Url.Action("Index", "Home", null, Request.Scheme),
                ["Named Route"] = Url.RouteUrl("products", new { action = "List" })
            };

            ViewBag.RouteInfo = ControllerContext.ToCtxString();
            return View(urls);
        }

        // Action specifically for testing MyDisplayRouteInfo as IActionResult
        [Route("test-route-info")]
        public IActionResult TestRouteInfo()
        {
            // این مستقیماً اطلاعات route را نمایش می‌دهد
            return ControllerContext.MyDisplayRouteInfo(msg: "Testing route info display");
        }

        [Route("test-route-info/{id}")]
        public IActionResult TestRouteInfoWithId(int id)
        {
            // این مستقیماً اطلاعات route را نمایش می‌دهد
            return ControllerContext.MyDisplayRouteInfo(id, $"Testing with ID: {id}");
        }
    }
}