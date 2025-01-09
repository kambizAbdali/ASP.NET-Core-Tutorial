namespace TDD.Models
{
    public class SumHelper
    {
        public int Sum(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            // Split numbers and convert to int  
            var numbers = input.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(n => int.TryParse(n, out var num) ? num : 0);

            return numbers.Sum();
        }
    }
}