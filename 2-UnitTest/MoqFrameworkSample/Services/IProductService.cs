using MoqFrameworkSample.Models;

namespace MoqFrameworkSample.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(int id);
        void AddProduct(Product product);
        int CountProduct { get; }

    }
}
