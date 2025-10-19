using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityCompleteProject.Models;
using IdentityCompleteProject.Services;

namespace IdentityCompleteProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserService _userService;

        public UsersController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var users = await _userService.GetUsersAsync(page, pageSize);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userService.GetUserRolesAsync(id);
            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = _roleManager.Roles.ToList();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                return BadRequest();
            }

            var result = await _userService.AddUserToRoleAsync(userId, roleName);
            if (result.Succeeded)
            {
                TempData["Success"] = $"User successfully added to {roleName} role.";
            }
            else
            {
                TempData["Error"] = $"Failed to add user to {roleName} role.";
            }

            return RedirectToAction("Details", new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                return BadRequest();
            }

            var result = await _userService.RemoveUserFromRoleAsync(userId, roleName);
            if (result.Succeeded)
            {
                TempData["Success"] = $"User successfully removed from {roleName} role.";
            }
            else
            {
                TempData["Error"] = $"Failed to remove user from {roleName} role.";
            }

            return RedirectToAction("Details", new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(7));
            if (result.Succeeded)
            {
                TempData["Success"] = "User account locked successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to lock user account.";
            }

            return RedirectToAction("Details", new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            if (result.Succeeded)
            {
                TempData["Success"] = "User account unlocked successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to unlock user account.";
            }

            return RedirectToAction("Details", new { id = userId });
        }
    }
}