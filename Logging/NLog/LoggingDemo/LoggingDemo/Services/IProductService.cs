using LoggingDemo.Models;

namespace LoggingDemo.Services
{
    public interface IProductService
    {
        Product CreateProduct(Product product);
        Product UpdateProduct(int id, Product product);
        List<Product> GetProducts();
        void SimulateCriticalError();
    }
}