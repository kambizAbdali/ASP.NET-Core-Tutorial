using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens; // Contains classes for handling security tokens.
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt; // Provides classes for handling JSON Web Tokens (JWT).
using System.Linq;
using System.Security.Claims; // Provides classes for representing claims-based identities.
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Interfaces.IRepositories;

namespace TaskManagement.Infrastructure.Repositories
{
    /// 
    /// JWT token service for token generation and validation
    /// 
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration; 
        private readonly IUserTokenRepository _tokenRepository; 

        public TokenService(IConfiguration configuration, IUserTokenRepository tokenRepository)
        {
            _configuration = configuration;
            _tokenRepository = tokenRepository;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler(); // Creates a JWT handler.
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor // Defines the structure of the JWT token.
            {
                Subject = new ClaimsIdentity(new[] // Sets the claims (data) stored in the token.
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Adds the user's ID as a claim.
                    new Claim(ClaimTypes.Name, user.Username), // Adds the user's username as a claim.
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty) // Adds the user's email as a claim.
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])), // Sets the token's expiration time.
                Issuer = _configuration["Jwt:Issuer"], // Sets the token's issuer (who issued the token). // Issuer: the entity that issues the token.
                Audience = _configuration["Jwt:Audience"], // Sets the token's audience (intended recipients/consumers of the token). // Audience: the intended recipients of the token.
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Configures the signing credentials using the secret key and hashing algorithm.
            };

            var token = tokenHandler.CreateToken(tokenDescriptor); // Creates the JWT token.
            return tokenHandler.WriteToken(token); // Serializes the token to a string.
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler(); // Creates a JWT handler.
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]); // Retrieves the secret key from configuration and encodes it.

            var validationParameters = new TokenValidationParameters // Configures the validation parameters for the token.
            {
                ValidateIssuerSigningKey = true, // Indicates that the issuer's signing key should be validated.
                IssuerSigningKey = new SymmetricSecurityKey(key), // Sets the issuer's signing key.
                ValidateIssuer = true, // Indicates that the issuer should be validated.
                ValidIssuer = _configuration["Jwt:Issuer"], // Sets the valid issuer.
                ValidateAudience = true, // Indicates that the audience should be validated.
                ValidAudience = _configuration["Jwt:Audience"], // Sets the valid audience.
                ValidateLifetime = true, // Indicates that the lifetime of the token should be validated.
                ClockSkew = TimeSpan.Zero // Sets the clock skew to zero (no tolerance for time differences).
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _); // Validates the token.
                return principal; // Returns the ClaimsPrincipal representing the token's claims.
            }
            catch
            {
                return null; // Returns null if the token is invalid.
            }
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32]; // Creates a byte array to store the random number.
            using (var rng = RandomNumberGenerator.Create()) 
            {
                rng.GetBytes(randomNumber); // Fills the byte array with random bytes.
                return Convert.ToBase64String(randomNumber); // Converts the random bytes to a base64 string.
            }
        }

        public async Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var token = await _tokenRepository.GetActiveTokenAsync(userId, refreshToken); // Retrieves an active token from the repository.
            return token != null; // Returns true if the token is found, false otherwise.
        }
    }
}