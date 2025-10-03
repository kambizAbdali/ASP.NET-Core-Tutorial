using AdvancedRoutingDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Docs.Samples;

namespace AdvancedRoutingDemo.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // Conventional Routing: /Product/List or /products
        public async Task<IActionResult> List(string category = null)
        {
            var products = await _context.Products
                .Where(p => category == null || p.Category == category)
                .ToListAsync();

            ViewBag.Category = category;
            ViewBag.RouteInfo = ControllerContext.ToCtxString(msg: $"Category: {category}");
            return View(products);
        }

        // Multiple routes with constraints
        [Route("product/{id:int}")]
        [Route("products/details/{id:int}")]
        [Route("item/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.RouteInfo = ControllerContext.ToCtxString(id, $"Product ID: {id}");
            return View(product);
        }

        // SEO-friendly URL with custom route
        [Route("catalog/{category}/{productName}")]
        public async Task<IActionResult> SeoDetails(string category, string productName)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.Category.ToLower() == category.ToLower() &&
                    p.Name.ToLower().Replace(" ", "-") == productName.ToLower());

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.RouteInfo = ControllerContext.ToCtxString(msg: $"Category: {category}, Product: {productName}");
            return View("Details", product);
        }

        // HTTP Verb based routing
        [HttpGet("product/create")]
        public IActionResult Create()
        {
            ViewBag.RouteInfo = ControllerContext.ToCtxString();
            return View();
        }

        [HttpPost("product/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = product.Id });
            }

            ViewBag.RouteInfo = ControllerContext.ToCtxString();
            return View(product);
        }

        // Route with order and name
        [Route("featured-products", Name = "FeaturedProducts", Order = 1)]
        public async Task<IActionResult> Featured()
        {
            var featuredProducts = await _context.Products
                .Take(3)
                .ToListAsync();

            ViewBag.RouteInfo = ControllerContext.ToCtxString();
            return View(featuredProducts);
        }

        // Using ~/ to override controller route prefix
        [Route("~/special-offers")]
        public async Task<IActionResult> SpecialOffers()
        {
            var offers = await _context.Products
                .Where(p => p.Price < 500)
                .ToListAsync();

            ViewBag.RouteInfo = ControllerContext.ToCtxString();
            return View(offers);
        }
    }
}