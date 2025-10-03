using Microsoft.AspNetCore.Mvc;
using Microsoft.Docs.Samples;

namespace AdvancedRoutingDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class DashboardController : Controller
    {
        // /Admin/Dashboard/Index
        public IActionResult Index()
        {
            ViewBag.CurrentRoute = ControllerContext.MyDisplayRouteInfo();
            return View();
        }

        // /Admin/Dashboard/Users
        [Route("Admin/Users")]
        public IActionResult Users()
        {
            ViewBag.CurrentRoute = ControllerContext.MyDisplayRouteInfo();
            return View();
        }

        // /Admin/Dashboard/Settings
        [Route("[area]/Settings")]
        public IActionResult Settings()
        {
            ViewBag.CurrentRoute = ControllerContext.MyDisplayRouteInfo();
            return View();
        }

        // Using ~/ to create root-level admin routes
        [Route("~/super-admin")]
        public IActionResult SuperAdmin()
        {
            ViewBag.CurrentRoute = ControllerContext.MyDisplayRouteInfo();
            return View();
        }
    }
}