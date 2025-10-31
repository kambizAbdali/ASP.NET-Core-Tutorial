using System;

namespace TaskManagement.Core.Entities
{
    /// <summary>
    /// Represents a user in the task management system
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        /// <summary>
        /// Store hashed password for security - never store plain text passwords
        /// </summary>
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// User status for authorization checks and soft deletion
        /// </summary>
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
    }
}