using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageDemo.Models;
using RazorPageDemo.Services;

namespace RazorPageDemo.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IProductService productService, ILogger<DeleteModel> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public Product Product { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _productService.GetByIdAsync(id) ?? new Product();

            if (Product.Id == 0)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Product deleted successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product ID: {ProductId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the product.";
                return RedirectToPage("./Index");
            }
        }
    }
}