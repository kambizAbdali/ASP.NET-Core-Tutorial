using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageDemo.Models;
using RazorPageDemo.Services;

namespace RazorPageDemo.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IProductService productService, ILogger<IndexModel> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public List<Product> Products { get; set; } = new();
        public string? SearchCategory { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync(string? category)
        {
            try
            {
                _logger.LogInformation("Loading products list, Category: {Category}", category ?? "All");

                if (!string.IsNullOrEmpty(category))
                {
                    SearchCategory = category;
                    Products = await _productService.GetByCategoryAsync(category);
                }
                else
                {
                    Products = await _productService.GetAllAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                ErrorMessage = "An error occurred while loading products.";
                Products = new List<Product>();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete product ID: {ProductId}", id);

                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    ErrorMessage = "Product not found!";
                    return RedirectToPage();
                }

                await _productService.DeleteAsync(id);
                SuccessMessage = $"Product '{product.Name}' deleted successfully!";

                _logger.LogInformation("Product deleted: {ProductName}", product.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product ID: {ProductId}", id);
                ErrorMessage = "An error occurred while deleting the product.";
            }

            return RedirectToPage();
        }
    }
}