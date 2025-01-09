using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TDD.Controllers;
using TDD.Models;

namespace TDD.Test
{
    public class HomeControllerTest
    {
        [Fact]
        public void IndexTest()
        {
            HomeController controller = new HomeController();
            var result= controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData("1,5", 6)]
        [InlineData("1, 5, 5", 11)]
        [InlineData("1, 5, 2, 4", 12)]
        [InlineData("0", 0)]
        [InlineData("1,5,", 6)]
        public void SumHelperTests(string input, int expected)
        {
            // Arrange  
            var sumHelper = new SumHelper();

            // Act  
            var result = sumHelper.Sum(input);

            // Assert  
            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData("1,5", 6)]
        [InlineData("1, 5, 5", 11)]
        [InlineData("1, 5, 2, 4", 12)]
        [InlineData("0", 0)]
        [InlineData("1,5,", 6)]
        public void CalculateSum_ReturnsCorrectJsonResult(string input, int expected)
        {
            // Arrange  
            var controller = new HomeController();

            // Act  
            var result = controller.CalculateSum(input) as JsonResult;

            // Assert  
            Assert.NotNull(result); // اطمینان حاصل کنید که نتیجه JSON است
                                    // 
            Assert.IsType<JsonResult>(controller.CalculateSum(input));
            var jsonResult = JObject.FromObject(result.Value); // استفاده از JObject  
            Assert.Equal(expected, jsonResult["sum"].Value<int>()); // مقایسه نتیجه  
        }
    }
}