using WebAppMVC.Models.Entites;

namespace WebAppMVC.Models.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
        Product GetById(long id);
        Product Add(Product product);
        void Remove(long id);
        Product Update(Product product);
    }
}
