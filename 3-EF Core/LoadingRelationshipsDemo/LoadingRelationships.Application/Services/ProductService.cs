using LoadingRelationships.Core.Entities;
using LoadingRelationships.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingRelationships.Application.Services
{
    public class ProductService: IProductRepository
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<List<Product>> GetAllProductsAsync(bool useNoTracking = false)
        {
            if (useNoTracking)
            {
                return await _productRepository.ListAllAsyncNoTracking();
            }
            return await _productRepository.ListAllAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = new Product { Id = id };
            await _productRepository.DeleteAsync(product);
        }

        public decimal CalculateTax(decimal price)
        {
            // منطق محاسبه مالیات
            return price * 0.09m; // ۹ درصد مالیات
        }

        public async Task<List<Product>> GetExpensiveProductsAsync()
        {
            //Example of when to use AsEnumerable
            var products = await _productRepository.ListAllAsync();
            return products.AsEnumerable().Where(p => CalculateTax(p.Price) > 100).ToList();
        }
    }
}