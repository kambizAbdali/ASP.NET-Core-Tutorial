using ConcurrencyExample.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyExample.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> GetProductAsync(long id);
        Task UpdateProductAsync(long id, string newName);
        Task CreateProductAsync(string name, decimal price);
        Task DeleteProductAsync(long id);
        Task PerformTransactionalOperationAsync(string name1, decimal price1, string name2, decimal price2);
        Task PerformMultipleEntityChangesAsync(long id1, string newName1, long id2, string newName2);
    }
}
