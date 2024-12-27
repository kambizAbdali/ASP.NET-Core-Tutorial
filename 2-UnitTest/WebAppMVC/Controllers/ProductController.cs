using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Models.Services;
using WebAppMVC.Models; // Ensure you have your Product model in the namespace  
using WebAppMVC.Models.Entites;

namespace WebAppMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: ProductController  
        public ActionResult Index()
        {
            var products = _productService.GetAll();
            return View(products); // Pass the list of products to the view  
        }

        // GET: ProductController/Details/5  
        public ActionResult Details(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound(); // Return a 404 if the product is not found  
            }
            return View(product); // Pass the product details to the view  
        }

        // GET: ProductController/Create  
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductController/Create  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _productService.Add(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product); // Return to the view with the model if validation fails  
        }

        // GET: ProductController/Edit/5  
        public ActionResult Edit(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: ProductController/Edit/5  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product)
        {
            if (ModelState.IsValid)
            {
                product.Id = id; // Ensure the product Id is set  
                _productService.Update(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: ProductController/Delete/5  
        public ActionResult Delete(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: ProductController/Delete/5  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _productService.Remove(id);
            return RedirectToAction(nameof(Index));
        }

   
    }
}