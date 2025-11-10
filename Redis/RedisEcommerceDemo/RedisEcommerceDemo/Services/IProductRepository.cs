using RedisEcommerceDemo.Models;

namespace RedisEcommerceDemo.Services
{
    public interface IProductRepository
    {
        Task<List<Product>> GetFeaturedProductsAsync();
        Task<List<Product>> GetBestSellingProductsAsync();
        Task<List<Product>> GetNewArrivalsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<bool> UpdateProductViewCountAsync(int productId);
        Task<HomePageViewModel> GetHomePageDataAsync();
    }
}