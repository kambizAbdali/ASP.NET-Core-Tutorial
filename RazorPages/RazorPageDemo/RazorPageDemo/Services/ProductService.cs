using RazorPageDemo.Models;
using RazorPageDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace RazorPageDemo.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<List<Product>> GetByCategoryAsync(string category);
    }

    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AppDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            try
            {
                return await _context.Products
                    .OrderBy(p => p.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                throw;
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by ID: {ProductId}", id);
                throw;
            }
        }

        public async Task CreateAsync(Product product)
        {
            try
            {
                product.CreatedDate = DateTime.Now;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product created: {ProductName} (ID: {ProductId})", product.Name, product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {ProductName}", product.Name);
                throw;
            }
        }

        public async Task UpdateAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct != null)
                {
                    // به روزرسانی فیلدها
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Description = product.Description;
                    existingProduct.Category = product.Category;
                    // تاریخ ایجاد تغییر نمی‌کند

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Product updated: {ProductName} (ID: {ProductId})", product.Name, product.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {ProductName}", product.Name);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Product deleted: {ProductName} (ID: {ProductId})", product.Name, id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<List<Product>> GetByCategoryAsync(string category)
        {
            try
            {
                return await _context.Products
                    .Where(p => p.Category.ToLower() == category.ToLower())
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by category: {Category}", category);
                throw;
            }
        }
    }
}