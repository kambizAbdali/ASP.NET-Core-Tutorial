using System.ComponentModel.DataAnnotations;

namespace TagHelpersDemo.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [Display(Name = "Product Name")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
        public decimal Price { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [Display(Name = "Product Status")]
        public ProductStatus Status { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "In Stock")]
        public bool InStock { get; set; }

        [Display(Name = "Stock Quantity")]
        [Range(0, 1000, ErrorMessage = "Stock quantity must be between 0 and 1000")]
        public int StockQuantity { get; set; }
    }

    public enum ProductStatus
    {
        [Display(Name = "Available")]
        Available,
        [Display(Name = "Out of Stock")]
        OutOfStock,
        [Display(Name = "Discontinued")]
        Discontinued,
        [Display(Name = "Coming Soon")]
        ComingSoon
    }
}