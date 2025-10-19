using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace IdentityCompleteProject.Models
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }


        [BindNever]
        public DateTime CreatedDate { get; set; }= DateTime.Now;
        
        [Required]
        [Display(Name = "National Code")]
        [StringLength(10)]
        public string NationalCode { get; set; }

        // Navigation properties
        public virtual ICollection<UserAddress> Addresses { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }

        // Computed property for full name
        public string FullName => $"{FirstName} {LastName}";
    }
}