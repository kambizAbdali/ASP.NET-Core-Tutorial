using LoggingDemo.Models;
using LoggingDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoggingDemo.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
            _logger.LogInformation("UsersController initialized");
        }

        public IActionResult Index()
        {
            _logger.LogDebug("Loading users list");
            var users = _userService.GetAllUsers();
            return View(users);
        }

        public IActionResult Create()
        {
            _logger.LogDebug("Displaying user creation form");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            _logger.LogInformation("Attempting to create new user: {UserEmail}", user.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("User creation failed - invalid model state. Errors: {ModelErrors}",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View(user);
            }

            try
            {
                var createdUser = _userService.CreateUser(user);
                _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);

                TempData["SuccessMessage"] = $"User {createdUser.FullName} created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user: {@User}", user);
                ModelState.AddModelError("", "An error occurred while creating the user.");
                return View(user);
            }
        }

        public IActionResult Details(int id)
        {
            _logger.LogDebug("Retrieving details for user ID: {UserId}", id);

            try
            {
                var user = _userService.GetUser(id);
                return View(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("User not found: {UserId} - {ErrorMessage}", id, ex.Message);
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);

            try
            {
                _userService.DeleteUser(id);
                _logger.LogInformation("User deleted successfully: {UserId}", id);

                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Delete failed: {ErrorMessage}", ex.Message);
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}