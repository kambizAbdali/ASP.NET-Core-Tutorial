namespace MoqFrameworkSample.Models
{
    // Product.cs  
    public class Product
    {
        public int Id { get; set; }  // Unique identifier for the product  
        public string Name { get; set; }  // Name of the product  
        public decimal Price { get; set; }  // Price of the product  
        public string Description { get; set; }  // Description of the product  
        public bool InStock { get; set; }
    }
}
