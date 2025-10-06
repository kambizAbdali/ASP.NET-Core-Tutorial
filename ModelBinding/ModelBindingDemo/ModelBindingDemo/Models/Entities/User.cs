using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ModelBindingDemo.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindNever]
        public DateTime CreatedAt { get; set; }

        [BindNever]
        public bool IsActive { get; set; } = true;
    }
}
