using DapperSample.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DapperSample.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CustomerService _customerService;

        public CustomersController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> DetailsWithOrdersAndAddress(int id)
        {
            var customer = await _customerService.GetCustomerWithOrdersAndAddressAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
    }
}