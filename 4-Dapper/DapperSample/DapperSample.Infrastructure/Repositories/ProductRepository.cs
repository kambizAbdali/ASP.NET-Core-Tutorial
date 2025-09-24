using Dapper;
using DapperSample.Core.DTOs;
using DapperSample.Core.Entities;
using DapperSample.Core.Interfaces;
using DapperSample.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperSample.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public ProductRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Products WHERE Id = @Id", new { Id = id });
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryAsync<Product>("SELECT * FROM Products");
        }

        public async Task<int> AddAsync(Product product)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            string sql = "INSERT INTO Products (Name, Price, CategoryId) VALUES (@Name, @Price, @CategoryId); SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.ExecuteScalarAsync<int>(sql, product);
        }

        public async Task<int> UpdateAsync(Product product)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            string sql = "UPDATE Products SET Name = @Name, Price = @Price, CategoryId = @CategoryId WHERE Id = @Id";
            return await connection.ExecuteAsync(sql, product);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM Products WHERE Id = @Id", new { Id = id });
        }

        // Map Dapper Query Result with DTO Class Model
        public async Task<IEnumerable<ProductDto>> GetProductDtosAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            string sql = @"SELECT p.Id, p.Name, p.Price, c.Name AS CategoryName
                         FROM Products p
                         INNER JOIN Categories c ON p.CategoryId = c.Id";

            return await connection.QueryAsync<ProductDto>(sql);
        }

        // N:N Relationship
        public async Task<IEnumerable<Product>> GetProductsWithTagsAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            string sql = @"SELECT p.*, t.*
                FROM Products p
                INNER JOIN ProductTag pt ON p.Id = pt.ProductId
                INNER JOIN Tags t ON pt.TagId = t.Id";

            var productDictionary = new Dictionary<int, Product>();

            var products = await connection.QueryAsync<Product, Tag, Product>(
                sql,
                (product, tag) =>
                {
                    if (!productDictionary.TryGetValue(product.Id, out var productEntry))
                    {
                        productEntry = product;
                        productEntry.Tags = new List<Tag>();
                        productDictionary.Add(productEntry.Id, productEntry);
                    }

                    productEntry.Tags.Add(tag);
                    return productEntry;
                },
                splitOn: "Id");

            return products.Distinct().ToList(); // Distinct for removing duplicate products
        }

        public async Task<Product> GetProductWithCategoryAsync(int productId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            string sql = @"SELECT p.*, c.*
                     FROM Products p
                     INNER JOIN Categories c ON p.CategoryId = c.Id
                     WHERE p.Id = @ProductId";

            var products = await connection.QueryAsync<Product, Category, Product>(
                sql,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                },
                new { ProductId = productId },
                splitOn: "Id");

            return products.FirstOrDefault(); // changed to FirstOrDefault()
        }
        public async Task UpdateProductPriceStoredProcedureAsync(int productId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var parameters = new { ProductId = productId };
            await connection.ExecuteAsync("sp_UpdateProductPrice", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}