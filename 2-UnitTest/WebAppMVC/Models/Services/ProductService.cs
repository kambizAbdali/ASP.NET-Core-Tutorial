using WebAppMVC.Models.Entites;

namespace WebAppMVC.Models.Services
{
    public class ProductService : IProductService
    {
        public readonly DatabaseContext _context;
        public ProductService(DatabaseContext context)
        {
            _context = context;
        }
        public Product Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public Product GetById(long id)
        {
            return _context.Products.Find(id);
        }

        public void Remove(long id)
        {
            var product = _context.Products.Find(id);
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public Product Update(Product product)
        {
            var existingProduct = _context.Products.Find(product.Id);
            if (existingProduct != null)
            {
                // Update the properties  
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;

                _context.SaveChanges(); // Save the changes to the database  
                return existingProduct; // Return the updated product  
            }
            return null; // Could return null if the product doesn't exist  
        }
    }
}
