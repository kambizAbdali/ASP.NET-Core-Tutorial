using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using TagHelpersDemo.Extensions;
using TagHelpersDemo.Models;
using TagHelpersDemo.Services;

namespace TagHelpersDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IProductService productService, IMemoryCache memoryCache, ILogger<HomeController> logger)
        {
            _productService = productService;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["WelcomeMessage"] = "Welcome to Tag Helpers Demo!";
            ViewData["CurrentTime"] = DateTime.Now.ToString("F");
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateViewData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productService.CreateProductAsync(product);
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateViewData();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            await PopulateViewData();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(product);
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateViewData();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> AdvancedForm()
        {
            await PopulateViewData();
            return View(new ProductViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AdvancedForm(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Advanced form submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateViewData();
            return View(model);
        }

        public IActionResult CacheDemo()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        private async Task PopulateViewData()
        {
            var categories = await _productService.GetCategoriesAsync();

            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            var groupedCategories = categories.GroupBy(c => c.Type)
                .SelectMany(g => g.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Group = new SelectListGroup { Name = g.Key.ToString() }
                })).ToList();
            ViewBag.GroupedCategories = groupedCategories;

            var statusList = Enum.GetValues(typeof(ProductStatus))
                .Cast<ProductStatus>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.GetDisplayName()
                }).ToList();
            ViewBag.StatusList = statusList;

            ViewBag.AllCategories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }
    }
}