using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SignalRChatApp.Controllers
{
    [Authorize(Roles = "Operator")]
    public class SupportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}