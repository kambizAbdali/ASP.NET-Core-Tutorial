namespace Inheritance.Models
{
    public class BankTransferPayment : Payment
    {
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
    }
}
