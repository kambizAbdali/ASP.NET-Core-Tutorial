using LoadingRelationships.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoadingRelationshipsDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("ByCity")]
        public async Task<IActionResult> GetUsersByCity([FromQuery] string city)
        {
            var users = await _userService.GetUsersByCityAsync(city);

            // Demonstrate ToQueryString (for generating query strings)
            var query = HttpContext.Request.QueryString.ToString();
            Console.WriteLine($"Query String: {query}");

            return Ok(users);
        }
    }
}