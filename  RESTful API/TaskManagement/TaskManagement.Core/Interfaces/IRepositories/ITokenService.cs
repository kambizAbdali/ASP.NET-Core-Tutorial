using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces.IRepositories
{
    /// <summary>
    /// Manages JWT token operations including generation and validation
    /// </summary>
    public interface ITokenService
    {
        string GenerateToken(User user);
        System.Security.Claims.ClaimsPrincipal ValidateToken(string token);
        string GenerateRefreshToken();
        Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken);
    }
}
