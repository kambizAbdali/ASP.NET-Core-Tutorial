namespace RazorPageDemo.Models
{
    public class CalculatorModel
    {
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public int Result { get; set; }
        public string Operation { get; set; } = string.Empty;
    }
}