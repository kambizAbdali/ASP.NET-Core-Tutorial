using Calculator.Interfaces;
using Calculator.Services;
using System.Collections;
using Xunit;
using Xunit.Abstractions;

namespace Calculator.Test
{
    public class AddData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 3, 4, 7 };
            yield return new object[] { 1, 9, 10 };
            yield return new object[] { -1, 1, 0 };
            yield return new object[] { -5, -5, -10 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class CalculatorTests
    {
        private readonly ICalculatorService _calculator;
        private readonly ITestOutputHelper _output;

        public CalculatorTests(ITestOutputHelper output)
        {
            _calculator = new CalculatorService(); // You can use DI here  
            _output = output;
        }

        [Theory]
        [InlineData(3, 4, 7)]
        [InlineData(1, 9, 10)]
        [Trait("Category", "Addition")]
        public void Add_ShouldReturnCorrectSum(int a, int b, int expected)
        {
            // Act  
            int result = _calculator.Add(a, b);

            // Log the result  
            _output.WriteLine($"Add({a}, {b}) = {result}, expected = {expected}");

            // Assert  
            Assert.Equal(expected, result);
            Assert.IsType<int>(result);
        }

        public static IEnumerable<object[]> AddData =>
           new List<object[]>
           {
                new object[] { 3, 4, 7 },
                new object[] { 1, 9, 10 },
                new object[] { -1, 1, 0 },
                new object[] { -5, -5, -10 }
           };

        [Theory]
        [MemberData(nameof(AddData))]
        [Trait("Category", "Addition")]
        public void Add_ShouldReturnCorrectSum_MemberData(int a, int b, int expected)
        {
            // Act  
            int result = _calculator.Add(a, b);

            // Log the result  
            _output.WriteLine($"Add({a}, {b}) = {result}, expected = {expected}");

            // Assert  
            Assert.Equal(expected, result);
            Assert.IsType<int>(result);
        }

        [Theory]
        [ClassData(typeof(AddData))]
        [Trait("Category", "Addition")]
        public void Add_ShouldReturnCorrectSum_ClassData(int a, int b, int expected)
        {
            // Act  
            var result = _calculator.Add(a, b);

            // Log the result  
            _output.WriteLine($"Add({a}, {b}) = {result}, expected = {expected}");

            // Assert  
            Assert.Equal(expected, result);
            Assert.IsType<int>(result);
        }

        [Trait("Category", "Basic Operations")]
        [Fact(Skip = "Skipping this test for now due to a known issue.")]
        public void Add_ShouldReturnCorrectResult()
        {
            var result = _calculator.Add(2, 3);
            _output.WriteLine($"Add(2, 3) = {result}, expected = 5");
            Assert.Equal(5, result);
        }

        [Fact]
        [Trait("Category", "Basic Operations")]
        public void Subtract_ShouldReturnCorrectResult()
        {
            var result = _calculator.Subtract(5, 3);
            _output.WriteLine($"Subtract(5, 3) = {result}, expected = 2");
            Assert.Equal(2, result);
        }

        [Fact]
        [Trait("Category", "Basic Operations")]
        public void Multiply_ShouldReturnCorrectResult()
        {
            var result = _calculator.Multiply(2, 3);
            _output.WriteLine($"Multiply(2, 3) = {result}, expected = 6");
            Assert.Equal(6, result);
        }

        [Fact]
        [Trait("Category", "Basic Operations")]
        public void Divide_ShouldReturnCorrectResult()
        {
            var result = _calculator.Divide(6, 3);
            _output.WriteLine($"Divide(6, 3) = {result}, expected = 2");
            Assert.Equal(2, result);
        }

        [Fact]
        [Trait("Category", "Basic Operations")]
        public void Divide_ByZero_ShouldThrowArgumentException()
        {
            _output.WriteLine("Testing Divide by Zero");
            Assert.Throws<DivideByZeroException>(() => _calculator.Divide(6, 0));
        }
    }
}