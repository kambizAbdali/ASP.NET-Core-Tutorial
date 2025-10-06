using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelBindingDemo.Models.Entities;
using ModelBindingDemo.Models.ViewModels;
using ModelBindingDemo.Models.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ModelBindingDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PersonForm()
        {
            var person = new Person
            {
                Address = new Address(),
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber { Type = "Mobile" },
                    new PhoneNumber { Type = "Home" }
                }
            };
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PersonForm(Person person)
        {
            if (!ModelState.IsValid)
            {
                return View(person);
            }

            person.CreatedAt = DateTime.Now;
            _context.People.Add(person);
            _context.SaveChanges();

            TempData["Success"] = "Information saved successfully";
            return RedirectToAction("PersonDetails", new { id = person.Id });
        }

        public IActionResult PersonDetails(int id)
        {
            var person = _context.People
                .Include(p => p.Address)
                .Include(p => p.PhoneNumbers)
                .FirstOrDefault(p => p.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        [HttpGet]
        public IActionResult OrderForm()
        {
            return View(new OrderViewModel
            {
                Customer = new CustomerInfo(),
                Order = new OrderInfo(),
                Payment = new PaymentInfo()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrderForm([Bind(Prefix = "Customer")] CustomerInfo customer,
                                     [Bind(Prefix = "Order")] OrderInfo order,
                                     [Bind(Prefix = "Payment")] PaymentInfo payment)
        {
            var model = new OrderViewModel
            {
                Customer = customer,
                Order = order,
                Payment = payment
            };

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newOrder = new Order
            {
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd-HHmmss}",
                OrderDate = DateTime.Now,
                TotalAmount = order.Quantity * order.Price,
                Status = "Pending",
                Customer = new Customer
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Phone = customer.Phone
                    
                }
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            TempData["OrderSuccess"] = $"Your order has been registered with number {newOrder.OrderNumber}";
            return RedirectToAction("OrderConfirmation", new { id = newOrder.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public IActionResult ProcessWithPrefix([Bind(Prefix = "user")] Person person)
        {
            return Json(new
            {
                Message = "Data received with prefix",
                Data = new
                {
                    person.FirstName,
                    person.LastName,
                    person.Age
                }
            });
        }

        [HttpPost]
        public IActionResult ComplexParameters(
            string[] tags,
            Dictionary<string, string> settings,
            IFormCollection formData,
            IFormFile mainFile,
            IFormFile[] additionalFiles)
        {
            var result = new
            {
                Tags = tags,
                Settings = settings,
                FormKeys = formData.Keys,
                MainFile = mainFile?.FileName,
                AdditionalFilesCount = additionalFiles?.Length,
                AdditionalData = new
                {
                    Username = formData["username"],
                    Email = formData["email"]
                }
            };

            return Json(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}