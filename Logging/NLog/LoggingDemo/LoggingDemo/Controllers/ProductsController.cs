using LoggingDemo.Models;
using LoggingDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoggingDemo.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;

        public ProductsController(ILogger<ProductsController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
            _logger.LogInformation("ProductsController initialized");
        }

        public IActionResult Index()
        {
            _logger.LogDebug("Loading products list");
            var products = _productService.GetProducts();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            _logger.LogInformation("Creating new product: {ProductName}", product.Name);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Product creation failed - invalid model state");
                return View(product);
            }

            try
            {
                var createdProduct = _productService.CreateProduct(product);
                _logger.LogInformation("Product created successfully: {@Product}", createdProduct);

                TempData["SuccessMessage"] = $"Product '{createdProduct.Name}' created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product: {@Product}", product);
                ModelState.AddModelError("", ex.Message);
                return View(product);
            }
        }

        public IActionResult Edit(int id)
        {
            _logger.LogDebug("Loading product for edit: {ProductId}", id);

            try
            {
                var products = _productService.GetProducts();
                var product = products.FirstOrDefault(p => p.Id == id);

                if (product == null)
                {
                    _logger.LogWarning("Product not found for edit: {ProductId}", id);
                    return NotFound();
                }

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product for edit: {ProductId}", id);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            _logger.LogInformation("Updating product ID: {ProductId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Product update failed - invalid model state");
                return View(product);
            }

            try
            {
                var updatedProduct = _productService.UpdateProduct(id, product);
                _logger.LogInformation("Product updated successfully: {@Product}", updatedProduct);

                TempData["SuccessMessage"] = $"Product '{updatedProduct.Name}' updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product ID {ProductId}: {@Product}", id, product);
                ModelState.AddModelError("", ex.Message);
                return View(product);
            }
        }
    }
}