using Dapper;
using DapperSample.Core.Entities;
using DapperSample.Core.Interfaces;
using DapperSample.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperSample.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public CategoryRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Category>("SELECT * FROM Categories WHERE Id = @Id", new { Id = id });
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryAsync<Category>("SELECT * FROM Categories");
        }

        public async Task<int> AddAsync(Category category)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            string sql = "INSERT INTO Categories (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.ExecuteScalarAsync<int>(sql, category);
        }

        public async Task<int> UpdateAsync(Category category)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            string sql = "UPDATE Categories SET Name = @Name WHERE Id = @Id";
            return await connection.ExecuteAsync(sql, category);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM Categories WHERE Id = @Id", new { Id = id });
        }
    }
}