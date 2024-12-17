using Calculator.Interfaces;
using Calculator.Services;
using Xunit;

namespace Calculator.Test
{
    public class CalculatorTests
    {
        private readonly ICalculatorService _calculator;

        public CalculatorTests()
        {
            _calculator = new CalculatorService(); // You can use DI here
        }

        [Theory]
        [InlineData(3,4,7)]
        [InlineData(1, 9, 10)]
        public void Add_ShouldReturnCorrectSum(int a, int b, int expected)
        {
            // Act
            int result = _calculator.Add(a, b);

            // Assert
            Assert.Equal(expected, result);
            Assert.IsType<int>(result);
        }

        [Fact]
        public void Add_ShouldReturnCorrectResult()
        {
            var result = _calculator.Add(2, 3);
            Assert.Equal(5, result);
        }

        [Fact]
        public void Subtract_ShouldReturnCorrectResult()
        {
            var result = _calculator.Subtract(5, 3);
            Assert.Equal(2, result);
        }

        [Fact]
        public void Multiply_ShouldReturnCorrectResult()
        {
            var result = _calculator.Multiply(2, 3);
            Assert.Equal(6, result);
        }

        [Fact]
        public void Divide_ShouldReturnCorrectResult()
        {
            var result = _calculator.Divide(6, 3);
            Assert.Equal(2, result);
        }

        [Fact]
        public void Divide_ByZero_ShouldThrowArgumentException()
        {
            Assert.Throws<DivideByZeroException>(() => _calculator.Divide(6, 0));
        }

        // Similar tests for Subtract, Multiply, and Divide
    }
}