using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageDemo.Models;
using RazorPageDemo.Services;

namespace RazorPageDemo.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IProductService productService, ILogger<CreateModel> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [BindProperty]
        public Product Product { get; set; } = new();

        public void OnGet()
        {
            _logger.LogInformation("Loading product creation form");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Attempting to create new product: {ProductName}", Product.Name);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Product creation failed - validation errors");
                return Page();
            }

            try
            {
                await _productService.CreateAsync(Product);
                _logger.LogInformation("Product created successfully: {ProductName} (ID: {ProductId})",
                    Product.Name, Product.Id);

                TempData["SuccessMessage"] = $"Product '{Product.Name}' created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {ProductName}", Product.Name);
                ModelState.AddModelError("", "An error occurred while creating the product.");
                return Page();
            }
        }

        public IActionResult OnPostCancel()
        {
            _logger.LogInformation("Product creation cancelled");
            return RedirectToPage("./Index");
        }
    }
}