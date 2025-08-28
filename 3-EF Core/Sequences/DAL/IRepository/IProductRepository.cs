using Sequences.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sequences.Data.IRepository
{
        public interface IProductRepository
        {
            Task<Product> GetByIdAsync(int id);
            Task<IEnumerable<Product>> GetAllAsync();
            Task AddAsync(Product product);
            Task UpdateAsync(Product product);
            Task DeleteAsync(int id);
        }
}
