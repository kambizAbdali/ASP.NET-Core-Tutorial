using System.ComponentModel.DataAnnotations;

namespace IdentityCompleteProject.Models
{
    public class UserAddress
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string AddressLine { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; }

        public bool IsPrimary { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
