using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelBindingDemo.Models.DTOs;
using ModelBindingDemo.Models.Data;
using ModelBindingDemo.Models.Entities;

namespace ModelBindingDemo.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = $"User {model.Username} registered successfully";
            return RedirectToAction("RegisterSuccess");
        }

        public IActionResult RegisterSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Success"] = "Password changed successfully";
            return RedirectToAction("ChangePasswordSuccess");
        }

        public IActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UploadProfilePicture()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(UploadProfilePictureDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePicture.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }

                TempData["Success"] = $"Profile picture {fileName} uploaded successfully";
            }

            return RedirectToAction("UploadSuccess");
        }

        public IActionResult UploadSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Search([FromQuery] string term, [FromQuery] int page = 1)
        {
            ViewBag.Term = term;
            ViewBag.Page = page;
            return View();
        }

        [HttpGet("/user/profile/{id}/{tab}")]
        public IActionResult Profile(int id, string tab = "info")
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserId = id;
            ViewBag.ActiveTab = tab;
            ViewBag.User = user;
            return View();
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }
    }
}