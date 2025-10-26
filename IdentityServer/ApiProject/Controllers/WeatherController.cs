// Controllers/WeatherController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        [Authorize(Policy = "WeatherRead")]  // نیاز به scope خواندن
        public IActionResult Get()
        {
            var rng = new Random();
            var weatherData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return Ok(weatherData);
        }

        [HttpPost]
        [Authorize(Policy = "WeatherWrite")]  // نیاز به scope نوشتن
        public IActionResult Post([FromBody] WeatherForecast forecast)
        {
            // در واقعیت: ذخیره داده‌ها در دیتابیس
            return Ok(new { message = "Weather data saved successfully", data = forecast });
        }

        [HttpGet("admin")]
        [Authorize(Policy = "AdminOnly")]  // نیاز به نقش admin
        public IActionResult GetAdminData()
        {
            return Ok(new { message = "This is admin-only data", secret = "Top secret weather information" });
        }

        [HttpGet("public")]
        [AllowAnonymous]  // دسترسی عمومی
        public IActionResult GetPublicData()
        {
            return Ok(new { message = "This is public weather information" });
        }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string? Summary { get; set; }
    }
}