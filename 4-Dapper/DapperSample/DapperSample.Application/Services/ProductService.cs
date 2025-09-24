using DapperSample.Core.DTOs;
using DapperSample.Core.Entities;
using DapperSample.Core.Interfaces;

namespace DapperSample.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> GetProductAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<int> AddProductAsync(Product product)
        {
            return await _productRepository.AddAsync(product);
        }

        public async Task<int> UpdateProductAsync(Product product)
        {
            return await _productRepository.UpdateAsync(product);
        }

        public async Task<int> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductDto>> GetProductDtosAsync()
        {
            return await _productRepository.GetProductDtosAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithTagsAsync()
        {
            return await _productRepository.GetProductsWithTagsAsync();
        }

        public async Task<Product> GetProductWithCategoryAsync(int productId)
        {
            return await _productRepository.GetProductWithCategoryAsync(productId);
        }

        public async Task UpdateProductPriceStoredProcedureAsync(int productId)
        {
            await _productRepository.UpdateProductPriceStoredProcedureAsync(productId);
        }
    }
}