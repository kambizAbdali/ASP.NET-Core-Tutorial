using System.ComponentModel.DataAnnotations;

namespace ModelBindingDemo.Models.ViewModels
{
    public class CustomerInfo
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
