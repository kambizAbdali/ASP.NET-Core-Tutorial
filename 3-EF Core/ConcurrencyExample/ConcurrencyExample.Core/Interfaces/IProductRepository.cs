using ConcurrencyExample.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyExample.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(long id);
        Task UpdateAsync(Product product);
        Task AddProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task PerformTransactionalOperationAsync(Product product1, Product product2);
        Task PerformMultipleEntityChangesAsync(Product product1, Product product2);
    }
}
