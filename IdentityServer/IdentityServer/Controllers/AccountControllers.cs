using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly TestUserStore _users;

        public AccountController(TestUserStore users)
        {
            _users = users;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            // بررسی کاربر
            var user = _users.FindByUsername(username);
            if (user != null && _users.ValidateCredentials(username, password))
            {
                // ایجاد claims
                var claims = new List<Claim>
                {
                    new Claim("sub", user.SubjectId),
                    new Claim("name", user.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? username),
                    new Claim("preferred_username", username)
                };

                // اضافه کردن سایر claims کاربر
                claims.AddRange(user.Claims);

                // ایجاد identity و principal
                var identity = new ClaimsIdentity(claims, "password");
                var principal = new ClaimsPrincipal(identity);

                // ورود کاربر
                await HttpContext.SignInAsync(principal);

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Error"] = "Invalid username or password";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}