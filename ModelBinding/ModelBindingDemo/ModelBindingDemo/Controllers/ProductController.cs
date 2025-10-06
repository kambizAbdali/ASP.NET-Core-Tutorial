using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelBindingDemo.Models.DTOs;
using ModelBindingDemo.ModelBinders;
using ModelBindingDemo.Models.Data;
using ModelBindingDemo.Models.Entities;

namespace ModelBindingDemo.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View(new ProductSearchDto());
        }

        [HttpPost]
        public IActionResult Search(ProductSearchDto search)
        {
            if (!ModelState.IsValid)
            {
                return View(search);
            }

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(p => p.Name.Contains(search.Keyword) ||
                                        p.Description.Contains(search.Keyword));
            }

            if (!string.IsNullOrEmpty(search.Category))
            {
                query = query.Where(p => p.Category == search.Category);
            }

            if (search.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= search.MinPrice.Value);
            }

            if (search.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= search.MaxPrice.Value);
            }

            var results = query.ToList();
            ViewBag.SearchResults = results;
            ViewBag.Search = search;

            return View(search);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var newProduct = new Product
            {
                Name = product.Name,
                Price = product.Price,
                Category = product.Category,
                Description = product.Description,
                InStock = product.InStock,
                CreatedAt = DateTime.Now
            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            TempData["Success"] = $"Product {product.Name} created successfully";
            return RedirectToAction("Details", new { id = newProduct.Id });
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult CreateWithDate(
            ProductDto product,
            [ModelBinder(BinderType = typeof(CustomDateModelBinder))]
            DateTime productionDate)
        {
            ViewBag.ProductionDate = productionDate;
            return View("Create", product);
        }

        [HttpPost]
        public IActionResult CreateWithTags(
            ProductDto product,
            [ModelBinder(BinderType = typeof(CsvArrayModelBinder))]
            string[] tags)
        {
            ViewBag.Tags = tags;
            return View("Create", product);
        }

        [HttpGet]
        public IActionResult GetWithHeader([FromHeader] string userAgent, [FromHeader(Name = "X-API-Key")] string apiKey)
        {
            return Json(new
            {
                UserAgent = userAgent,
                ApiKey = apiKey,
                Message = "Data received from headers"
            });
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            _context.Products.Update(product);
            _context.SaveChanges();

            TempData["Success"] = "Product updated successfully";
            return RedirectToAction("Details", new { id = product.Id });
        }
    }
}