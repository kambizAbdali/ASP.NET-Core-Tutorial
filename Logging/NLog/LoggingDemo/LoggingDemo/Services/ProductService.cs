using LoggingDemo.Models;
using Microsoft.Extensions.Logging;

namespace LoggingDemo.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private static List<Product> _products = new();
        private static int _nextId = 1;

        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger;
            _logger.LogInformation("ProductService initialized");
        }

        public Product CreateProduct(Product product)
        {
            _logger.LogTrace("Entering CreateProduct method");

            try
            {
                // Validate product
                if (product.Price <= 0)
                {
                    _logger.LogWarning("Attempt to create product with invalid price: {Price}", product.Price);
                    throw new ArgumentException("Price must be greater than zero");
                }

                if (product.Stock < 0)
                {
                    _logger.LogWarning("Attempt to create product with negative stock: {Stock}", product.Stock);
                    throw new ArgumentException("Stock cannot be negative");
                }

                product.Id = _nextId++;
                _products.Add(product);

                // Log different levels based on product price
                if (product.Price > 1000)
                {
                    _logger.LogInformation("High-value product created: {ProductName} - ${Price}",
                        product.Name, product.Price);
                }
                else
                {
                    _logger.LogDebug("Product created: {@Product}", product);
                }

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {@Product}", product);
                throw;
            }
        }

        public Product UpdateProduct(int id, Product product)
        {
            _logger.LogDebug("Updating product ID: {ProductId}", id);

            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                _logger.LogError("Update failed: Product with ID {ProductId} not found", id);
                throw new ArgumentException($"Product with ID {id} not found");
            }

            // Log changes
            if (existingProduct.Price != product.Price)
            {
                _logger.LogInformation("Product price changed from {OldPrice} to {NewPrice}",
                    existingProduct.Price, product.Price);
            }

            if (existingProduct.Stock != product.Stock)
            {
                _logger.LogInformation("Product stock changed from {OldStock} to {NewStock}",
                    existingProduct.Stock, product.Stock);
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.Category = product.Category;

            _logger.LogInformation("Product updated successfully: {@Product}", existingProduct);
            return existingProduct;
        }

        public List<Product> GetProducts()
        {
            _logger.LogTrace("Retrieving all products");
            return _products;
        }

        public void SimulateCriticalError()
        {
            // Simulate a critical system error
            _logger.LogCritical("CRITICAL ERROR: Database connection failed! Application stability compromised.");

            // Simulate additional context
            var errorContext = new
            {
                DatabaseServer = "SQL-SERVER-01",
                ConnectionString = "Server=db-server;Database=ProductionDB;",
                Timestamp = DateTime.Now,
                ErrorCode = "DB_CONN_001"
            };

            _logger.LogCritical("Critical error details: {@ErrorContext}", errorContext);
        }
    }
}