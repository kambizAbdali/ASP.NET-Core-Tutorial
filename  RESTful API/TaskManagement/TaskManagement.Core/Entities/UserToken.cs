namespace TaskManagement.Core.Entities
{
    /// <summary>
    /// Manages JWT refresh tokens for users
    /// </summary>
    public class UserToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        /// <summary>
        /// The refresh token string - stored as GUID for uniqueness
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Expiration date of the refresh token
        /// Typically longer-lived than access token
        /// </summary>
        public DateTime RefreshTokenExpiry { get; set; }

        /// <summary>
        /// Indicates if the token has been used or revoked
        /// Security measure to prevent token reuse
        /// </summary>
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual User User { get; set; }
    }
}