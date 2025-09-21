using ConcurrencyExample.Application.Interfaces;
using ConcurrencyExample.Core.Entities;
using ConcurrencyExample.Core.Interfaces;
using ConcurrencyExample.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyExample.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> GetProductAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }

        public async Task UpdateProductAsync(long id, string newName)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new Exception("Product not found");

            product.Name = newName;
            await _productRepository.UpdateAsync(product);
        }

        public async Task CreateProductAsync(string name, decimal price)
        {
            var product = new Product
            {
                Name = name,
                Price = price
            };
            await _productRepository.AddProductAsync(product);
        }

        public async Task DeleteProductAsync(long id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new Exception("Product not found");

            await _productRepository.DeleteProductAsync(product);
        }

        public async Task PerformTransactionalOperationAsync(string name1, decimal price1, string name2, decimal price2)
        {
            var product1 = new Product { Name = name1, Price = price1 };
            var product2 = new Product { Name = name2, Price = price2 };

            await _productRepository.PerformTransactionalOperationAsync(product1, product2);
        }

        public async Task PerformMultipleEntityChangesAsync(long id1, string newName1, long id2, string newName2)
        {
            var product1 = await _productRepository.GetByIdAsync(id1);
            if (product1 == null) throw new Exception("Product 1 not found");

            var product2 = await _productRepository.GetByIdAsync(id2);
            if (product2 == null) throw new Exception("Product 2 not found");

            product1.Name = newName1;
            product2.Name = newName2;

            await _productRepository.PerformMultipleEntityChangesAsync(product1, product2);
        }
    }
}
