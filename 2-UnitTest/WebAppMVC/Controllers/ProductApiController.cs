using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Models.Entites;
using WebAppMVC.Models.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAppMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductApiController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/<ProductApiController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_productService.GetAll());
        }

        // GET api/<ProductApiController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_productService.GetById(id));
        }

        // POST api/<ProductApiController>
        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            return Ok(_productService.Add(product));
        }

        // PUT api/<ProductApiController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null || id != updatedProduct.Id)
            {
                return BadRequest("Product ID mismatch or invalid product.");
            }

            var existingProduct = _productService.GetById(id);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            // Update the existing product's properties  
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Description = updatedProduct.Description;
            // Add more properties as needed  

            // Save the updated product  
            _productService.Update(existingProduct);

            return NoContent(); // 204 No Content, indicating successful update  
        }

        // DELETE api/<ProductApiController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _productService.Remove(id);
            return Ok(true);
        }
    }
}