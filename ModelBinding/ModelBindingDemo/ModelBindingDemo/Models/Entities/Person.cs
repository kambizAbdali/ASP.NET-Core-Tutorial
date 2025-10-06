using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ModelBindingDemo.Models.Entities
{
    public class Person
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters")]
        public string LastName { get; set; }

        [Range(1, 140, ErrorMessage = "Age must be between 1 and 140")]
        public int Age { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public Address Address { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();

        [BindNever]
        public DateTime CreatedAt { get; set; }

        [BindNever]
        public bool IsActive { get; set; } = true;
    }

    public class Address
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        public string Street { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Postal code must be 10 digits")]
        public string PostalCode { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }
    }

    public class PhoneNumber
    {
        public int Id { get; set; }
        public string Type { get; set; } // Home, Mobile, Work
        public string Number { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}