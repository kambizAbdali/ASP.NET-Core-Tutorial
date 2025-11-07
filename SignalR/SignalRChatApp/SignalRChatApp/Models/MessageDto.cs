namespace SignalRChatApp.Models
{
    public class MessageDto
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public Guid RoomId { get; set; }
    }
}