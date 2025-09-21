using LoadingRelationships.Application.Services;
using LoadingRelationships.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LoadingRelationships.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(bool noTracking = false)
        {
            return await _productService.GetAllProductsAsync(noTracking);
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            await _productService.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            await _productService.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpGet("expensive")]
        public async Task<ActionResult<IEnumerable<Product>>> GetExpensiveProducts()
        {
            return await _productService.GetExpensiveProductsAsync();
        }
    }
}