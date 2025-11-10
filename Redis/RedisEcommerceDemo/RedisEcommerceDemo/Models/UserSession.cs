namespace RedisEcommerceDemo.Models
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        public List<string> Permissions { get; set; } = new List<string>();
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
    }
}
