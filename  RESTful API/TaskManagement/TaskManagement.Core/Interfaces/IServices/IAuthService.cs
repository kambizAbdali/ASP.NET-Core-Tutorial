using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs;

namespace TaskManagement.Core.Interfaces.IServices
{
    /// <summary>
    /// Service interface for authentication and authorization
    /// </summary>
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> ValidateUserAsync(string username, string password);
    }
}
