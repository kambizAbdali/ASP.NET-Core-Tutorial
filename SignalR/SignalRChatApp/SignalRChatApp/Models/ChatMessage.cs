using System.ComponentModel.DataAnnotations;

namespace SignalRChatApp.Models
{
    public class ChatMessage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime Time { get; set; }

        public bool IsRead { get; set; }

        [Required]
        public Guid ChatRoomId { get; set; }

        // Navigation property
        public virtual ChatRoom ChatRoom { get; set; }
    }
}