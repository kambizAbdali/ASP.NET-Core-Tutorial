namespace IdentityCompleteProject.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public string ProductName { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
