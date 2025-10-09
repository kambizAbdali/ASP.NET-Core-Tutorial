using System.ComponentModel.DataAnnotations;

namespace TagHelpersDemo.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Status")]
        public ProductStatus Status { get; set; }

        public List<int> SelectedCategories { get; set; } = new();
    }
}   