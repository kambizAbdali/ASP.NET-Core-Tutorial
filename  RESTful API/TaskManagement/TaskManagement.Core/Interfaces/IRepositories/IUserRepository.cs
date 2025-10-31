using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    /// <summary>
    /// User-specific repository for user management operations
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> ValidateUserAsync(string username, string password);
        Task<bool> UsernameExistsAsync(string username);
    }
}
