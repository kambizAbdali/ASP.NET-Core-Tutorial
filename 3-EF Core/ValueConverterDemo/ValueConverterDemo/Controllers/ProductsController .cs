using Microsoft.AspNetCore.Mvc;
using ValueConverterDemo.Core.Models;
using ValueConverterDemo.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ValueConverterDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }
    }
}