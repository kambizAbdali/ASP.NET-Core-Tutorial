using System.ComponentModel.DataAnnotations;

namespace ModelBindingDemo.Models.ViewModels
{
    public class OrderInfo
    {
        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; } = 1;

        [Range(0.01, 10000, ErrorMessage = "Invalid price")]
        public decimal Price { get; set; }

        public string Notes { get; set; }
    }
}
