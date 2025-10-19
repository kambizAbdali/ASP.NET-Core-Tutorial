using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPageDemo.Pages
{
    public class ProfileModel : PageModel
    {
        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPostUpdateProfile()
        {
            Message = "Profile updated successfully!";
            return Page();
        }

        public IActionResult OnPostChangePassword()
        {
            Message = "Password changed successfully!";
            return Page();
        }
    }
}