namespace ModelBindingDemo.Models.DTOs
{
    public class BulkProductsDto
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public string ImportType { get; set; }
    }
}
