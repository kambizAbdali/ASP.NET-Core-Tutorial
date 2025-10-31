using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    /// <summary>
    /// Generic repository pattern for data access operations
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveAsync();
    }
}