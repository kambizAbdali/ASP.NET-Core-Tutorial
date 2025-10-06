using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelBindingDemo.Models.DTOs;
using ModelBindingDemo.Models.Entities;
using ModelBindingDemo.Models.Data;
using ModelBindingDemo.ModelBinders;

namespace ModelBindingDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json", "application/xml")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] ProductSearchDto search)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(p => p.Name.Contains(search.Keyword));
            }

            if (!string.IsNullOrEmpty(search.Category))
            {
                query = query.Where(p => p.Category == search.Category);
            }

            var totalCount = query.Count();
            var products = query
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var results = new
            {
                Search = search,
                Products = products,
                TotalCount = totalCount,
                Page = search.Page,
                PageSize = search.PageSize
            };

            return Ok(results);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Invalid input data",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            if (_context.Users.Any(u => u.Username == register.Username))
            {
                return Conflict(new { Message = "Username already exists" });
            }

            var user = new User
            {
                Username = register.Username,
                Email = register.Email,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var result = new
            {
                Message = "Registration successful",
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                RegisteredAt = user.CreatedAt
            };

            return Ok(result);
        }

        [HttpPost("register-xml")]
        [Consumes("application/xml")]
        public IActionResult RegisterFromXml([FromBody] RegisterDto register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = new
            {
                Message = "XML registration successful",
                UserId = new Random().Next(1000, 9999),
                Username = register.Username,
                RegisteredAt = DateTime.Now
            };

            return Ok(result);
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id, [FromQuery] string format = "json")
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            return Ok(user);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(
            [FromForm] string description,
            [FromForm] IFormFile file,
            [FromForm] IFormFile[] attachments)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Main file is required" });
            }

            var result = new
            {
                OriginalFile = new
                {
                    FileName = file.FileName,
                    Size = file.Length,
                    ContentType = file.ContentType
                },
                Description = description,
                Attachments = attachments?.Select(f => new
                {
                    FileName = f.FileName,
                    Size = f.Length
                }),
                UploadedAt = DateTime.Now
            };

            return Ok(result);
        }

        [HttpGet("headers")]
        public IActionResult GetHeaders(
            [FromHeader] string userAgent,
            [FromHeader(Name = "Accept")] string accept,
            [FromHeader(Name = "Authorization")] string authorization)
        {
            var headers = new
            {
                UserAgent = userAgent,
                AcceptHeader = accept,
                AuthorizationHeader = authorization?.Substring(0, Math.Min(10, authorization.Length)) + "...",
                ReceivedAt = DateTime.Now
            };

            return Ok(headers);
        }

        [HttpPost("complex")]
        public IActionResult ComplexModel([FromBody] Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            person.CreatedAt = DateTime.Now;
            _context.People.Add(person);
            _context.SaveChanges();

            return Ok(new
            {
                Message = "Complex data received successfully",
                Person = person
            });
        }

        [HttpGet("csv-tags")]
        public IActionResult GetCsvTags(
            [ModelBinder(BinderType = typeof(CsvArrayModelBinder))]
            string[] tags)
        {
            return Ok(new
            {
                Tags = tags,
                Count = tags?.Length ?? 0
            });
        }

        [HttpGet("date-test")]
        public IActionResult TestDate(
            [ModelBinder(BinderType = typeof(CustomDateModelBinder))]
            DateTime date)
        {
            return Ok(new
            {
                InputDate = date,
                FormattedDate = date.ToString("yyyy-MM-dd"),
                DayOfWeek = date.DayOfWeek
            });
        }

        [HttpPost("strict-json")]
        [Consumes("application/json")]
        public IActionResult StrictJsonOnly([FromBody] ProductDto product)
        {
            return Ok(new
            {
                Message = "JSON data received successfully",
                Product = product
            });
        }
    }
}