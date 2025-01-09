using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TDD.Models;

namespace TDD.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult CalculateSum(string input)
        {
            var sumHelper = new SumHelper();
            var result = sumHelper.Sum(input);
            return Json(new { sum = result });
        }
    }
}