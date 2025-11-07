using System.ComponentModel.DataAnnotations;

namespace SignalRChatApp.Models
{
    public class ChatRoom
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string ConnectionId { get; set; }

        [Required]
        public string UserIdentifier { get; set; }

        [Required]
        public string GroupName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastActivity { get; set; } = DateTime.UtcNow;

        public int UnreadMessages { get; set; }

        // Navigation property for messages
        public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}