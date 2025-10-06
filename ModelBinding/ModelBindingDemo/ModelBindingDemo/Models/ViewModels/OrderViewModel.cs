using Microsoft.AspNetCore.Mvc;

namespace ModelBindingDemo.Models.ViewModels
{
    public class OrderViewModel
    {
        public CustomerInfo Customer { get; set; }
        public OrderInfo Order { get; set; }
        public PaymentInfo Payment { get; set; }
    }
}
