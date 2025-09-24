using DapperSample.Core.DTOs;
using DapperSample.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperSample.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<int> AddAsync(Product product);
        Task<int> UpdateAsync(Product product);
        Task<int> DeleteAsync(int id);

        Task<IEnumerable<ProductDto>> GetProductDtosAsync(); // Example DTO
        Task<IEnumerable<Product>> GetProductsWithTagsAsync(); // N:N
        Task<Product> GetProductWithCategoryAsync(int productId); // 1:N

        Task CallStoredProcedureAsync(int productId);
    }
}
