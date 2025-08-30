using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueConverterDemo.Core.Models
{
    // Represents the preferred shipping method for a product.
    public enum ShippingPreference
    {
        Standard,    // Standard shipping (e.g., 5-7 business days).
        Express,     // Express shipping (e.g., 2-3 business days).
        Priority     // Priority shipping (e.g., next-day delivery).
    }

    // Represents a product for sale.
    public class Product
    {
        [Key]  // Indicates this property is the primary key.
        public int ProductId { get; set; }

        [Required] //Indicates this property is the required.
        public string? ProductName { get; set; }

        [Required] //Indicates this property is the required.
        public decimal Price { get; set; }

        // The product's shipping preference, stored as a ShippingPreference enum.
        public ShippingPreference PreferredShipping { get; set; }
    }
}
