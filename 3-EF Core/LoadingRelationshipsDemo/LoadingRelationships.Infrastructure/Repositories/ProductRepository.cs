using LoadingRelationships.Core.Entities;
using LoadingRelationships.Core.Interfaces;
using LoadingRelationships.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingRelationships.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _dbContext.Products.FindAsync(id); // Find
        }

        public async Task<List<Product>> ListAllAsync()
        {
            return await _dbContext.Products.ToListAsync(); // Tracking
        }

        public async Task<List<Product>> ListAllAsyncNoTracking()
        {
            return await _dbContext.Products.AsNoTracking().ToListAsync(); // NoTracking
        }

        public async Task AddAsync(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            // حذف منطقی
            var productToDelete = await _dbContext.Products.FindAsync(product.Id);
            if (productToDelete != null)
            {
                productToDelete.IsRemoved = true;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}