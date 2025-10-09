using System.ComponentModel.DataAnnotations;

namespace TagHelpersDemo.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();

        [Display(Name = "Category Type")]
        public CategoryType Type { get; set; }
    }

    public enum CategoryType
    {
        [Display(Name = "Electronics")]
        Electronics,
        [Display(Name = "Clothing")]
        Clothing,
        [Display(Name = "Books")]
        Books,
        [Display(Name = "Home & Garden")]
        HomeGarden
    }
}