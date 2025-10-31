using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{

    /// <summary>
    /// Manages user token operations for refresh token functionality
    /// </summary>
    public interface IUserTokenRepository : IRepository<UserToken>
    {
        Task<UserToken> GetActiveTokenAsync(int userId, string refreshToken);
        Task DeactivateUserTokensAsync(int userId);
        Task CleanupExpiredTokensAsync();
    }
}
