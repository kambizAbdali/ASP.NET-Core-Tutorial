namespace Inheritance.Models
{
    public class CreditCardPayment : Payment
    {
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string Cvv { get; set; }
    }
}