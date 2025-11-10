namespace RedisEcommerceDemo.Models
{
    public class ShoppingCart
    {
        public int UserId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}