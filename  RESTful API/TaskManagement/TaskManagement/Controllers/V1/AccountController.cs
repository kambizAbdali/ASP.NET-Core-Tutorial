using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Interfaces.IRepositories;
using TaskManagement.Core.Interfaces.IServices;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.API.Controllers.V1
{
    /// <summary>
    /// Authentication controller for user registration, login, and token management
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserTokenRepository _tokenRepository;
        private readonly IConfiguration _configuration;

        public AccountController(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IUserTokenRepository tokenRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _tokenRepository = tokenRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// User registration endpoint
        /// </summary>
        /// <param name="registerDto">User registration data</param>
        /// <response code="201">User successfully created</response>
        /// <response code="400">If username already exists or input is invalid</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Check if username already exists
            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
            {
                return BadRequest(new { Message = "Username already exists" });
            }

            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = _passwordHasher.HashPassword(registerDto.Password),
                Email = registerDto.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            return CreatedAtAction(nameof(Login), new { message = "User registered successfully" });
        }

        /// <summary>
        /// User login endpoint with JWT token generation
        /// </summary>
        /// <param name="loginDto">User credentials</param>
        /// <response code="200">Returns access and refresh tokens</response>
        /// <response code="401">If credentials are invalid</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            // Validate user exists and password is correct
            if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            // Check if user account is active
            if (!user.IsActive)
            {
                return Unauthorized(new { Message = "Account is deactivated" });
            }

            // Generate tokens
            var accessToken = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Store refresh token in database
            var userToken = new UserToken
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpireDays"])),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _tokenRepository.AddAsync(userToken);
            await _tokenRepository.SaveAsync();

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                Username = user.Username
            };

            return Ok(response);
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="refreshTokenDto">Refresh token data</param>
        /// <response code="200">Returns new access and refresh tokens</response>
        /// <response code="401">If refresh token is invalid</response>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var principal = _tokenService.ValidateToken(refreshTokenDto.RefreshToken);
            if (principal == null)
            {
                return Unauthorized(new { Message = "Invalid refresh token" });
            }

            var userId = int.Parse(principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var isValid = await _tokenService.ValidateRefreshTokenAsync(userId, refreshTokenDto.RefreshToken);

            if (!isValid)
            {
                return Unauthorized(new { Message = "Invalid or expired refresh token" });
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return Unauthorized(new { Message = "User not found or inactive" });
            }

            // Generate new tokens
            var newAccessToken = _tokenService.GenerateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Update refresh token in database
            var userToken = await _tokenRepository.GetActiveTokenAsync(userId, refreshTokenDto.RefreshToken);
            if (userToken != null)
            {
                userToken.IsActive = false; // Invalidate old refresh token
                _tokenRepository.Update(userToken);
            }

            // Store new refresh token
            var newUserToken = new UserToken
            {
                UserId = user.Id,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpireDays"])),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _tokenRepository.AddAsync(newUserToken);
            await _tokenRepository.SaveAsync();

            var response = new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                Username = user.Username
            };

            return Ok(response);
        }

        /// <summary>
        /// Logout endpoint - invalidates refresh tokens
        /// </summary>
        /// <response code="200">Successfully logged out</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            // Deactivate all refresh tokens for the user
            await _tokenRepository.DeactivateUserTokensAsync(userId);

            return Ok(new { Message = "Successfully logged out" });
        }
    }
}