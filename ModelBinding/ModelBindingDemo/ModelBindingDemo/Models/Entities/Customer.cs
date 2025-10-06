using System.ComponentModel.DataAnnotations;

namespace ModelBindingDemo.Models.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public string Phone { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
