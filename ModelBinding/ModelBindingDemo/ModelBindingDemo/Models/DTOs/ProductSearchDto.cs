using System.ComponentModel.DataAnnotations;

namespace ModelBindingDemo.Models.DTOs
{
    public class ProductSearchDto
    {
        public string Keyword { get; set; }

        [Range(1, 1000, ErrorMessage = "Page must be between 1 and 1000")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        public string Category { get; set; }

        [Range(0, 10000, ErrorMessage = "Minimum price is invalid")]
        public decimal? MinPrice { get; set; }

        [Range(0, 10000, ErrorMessage = "Maximum price is invalid")]
        public decimal? MaxPrice { get; set; }
    }
}
