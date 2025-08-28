using System.ComponentModel.DataAnnotations;

namespace Sequences.Models
{
 public class Product : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
