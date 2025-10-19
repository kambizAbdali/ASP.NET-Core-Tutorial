using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageDemo.Models;
using RazorPageDemo.Services;

namespace RazorPageDemo.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IProductService productService, ILogger<EditModel> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [BindProperty]
        public Product Product { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            Product = product;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _productService.UpdateAsync(Product);
                TempData["SuccessMessage"] = $"Product '{Product.Name}' updated successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {ProductName}", Product.Name);
                ModelState.AddModelError("", "An error occurred while updating the product.");
                return Page();
            }
        }
    }
}