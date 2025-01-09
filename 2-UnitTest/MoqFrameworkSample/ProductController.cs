using Microsoft.AspNetCore.Mvc;
using MoqFrameworkSample.Models;
using MoqFrameworkSample.Services;

namespace MoqFrameworkSample
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            var products = _productService.GetAllProducts();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost] // Assuming this action is reached via a POST request  
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _productService.AddProduct(product);
                return RedirectToAction("Index"); // Redirect to the Index action after adding the product  
            }
            return View(product); // Return the view with the model if there's a validation error  
        }
    }
}