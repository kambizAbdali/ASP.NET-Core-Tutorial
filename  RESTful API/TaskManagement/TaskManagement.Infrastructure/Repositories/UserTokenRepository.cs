using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    /// <summary>
    /// User token repository for refresh token management
    /// </summary>
    public class UserTokenRepository : Repository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(TaskManagementContext context) : base(context)
        {
        }

        public async Task<UserToken> GetActiveTokenAsync(int userId, string refreshToken)
        {
            return await _dbSet
                .FirstOrDefaultAsync(ut => ut.UserId == userId &&
                                         ut.RefreshToken == refreshToken &&
                                         ut.IsActive &&
                                         ut.RefreshTokenExpiry > DateTime.UtcNow);
        }

        public async Task DeactivateUserTokensAsync(int userId)
        {
            var tokens = await _dbSet
                .Where(ut => ut.UserId == userId && ut.IsActive)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _dbSet
                .Where(ut => ut.RefreshTokenExpiry <= DateTime.UtcNow || !ut.IsActive)
                .ToListAsync();

            _dbSet.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}
