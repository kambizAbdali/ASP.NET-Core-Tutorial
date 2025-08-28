namespace Inheritance.Models
{
    public abstract class Payment
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}