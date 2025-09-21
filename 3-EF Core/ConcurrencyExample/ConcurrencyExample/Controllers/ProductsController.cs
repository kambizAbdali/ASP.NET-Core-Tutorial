using ConcurrencyExample.Application.Interfaces;
using ConcurrencyExample.Core.Models;
using ConcurrencyExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConcurrencyExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(long id)
        {
            var product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null || string.IsNullOrEmpty(updateProductDto.NewName))
            {
                return BadRequest("NewName is required");
            }

            try
            {
                await _productService.UpdateProductAsync(id, updateProductDto.NewName);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Concurrency conflict occurred. Please try again.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the product.");
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (createProductDto == null || string.IsNullOrEmpty(createProductDto.Name))
            {
                return BadRequest("Name is required");
            }

            try
            {
                await _productService.CreateProductAsync(createProductDto.Name, createProductDto.Price);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the product.");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the product.");
            }

            return NoContent();
        }

        [HttpPost("transactional")]
        public async Task<IActionResult> PerformTransactionalOperation([FromBody] TransactionalOperationDto dto)
        {
            try
            {
                await _productService.PerformTransactionalOperationAsync(dto.Name1, dto.Price1, dto.Name2, dto.Price2);
                return Ok("Transactional operation completed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Transactional operation failed: {ex.Message}");
            }
        }

        [HttpPut("multiple")]
        public async Task<IActionResult> PerformMultipleEntityChanges([FromBody] MultipleEntityChangesDto dto)
        {
            try
            {
                await _productService.PerformMultipleEntityChangesAsync(dto.Id1, dto.NewName1, dto.Id2, dto.NewName2);
                return Ok("Multiple entity changes completed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Multiple entity changes failed: {ex.Message}");
            }
        }
    }
}