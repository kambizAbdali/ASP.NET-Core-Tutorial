using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilterDemo.Filters.ExceptionFilter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            // Handle all unhandled exceptions
            // مدیریت تمام خطاهای پردازش نشده
            _logger.LogError(context.Exception, "🔥 Global exception filter caught an exception");

            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred",
                Instance = context.HttpContext.Request.Path,
                Status = StatusCodes.Status500InternalServerError
            };

            // Show detailed errors in development
            // نمایش خطاهای دقیق در محیط توسعه
            if (_env.IsDevelopment())
            {
                problemDetails.Detail = context.Exception.ToString();
                problemDetails.Extensions.Add("stackTrace", context.Exception.StackTrace);
            }
            else
            {
                problemDetails.Detail = "An internal server error occurred";
            }

            // Handle specific exception types
            // مدیریت انواع خاص خطاها
            switch (context.Exception)
            {
                case ArgumentNullException:
                case ArgumentException:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Bad Request";
                    break;

                case KeyNotFoundException:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Not Found";
                    break;

                case UnauthorizedAccessException:
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    problemDetails.Title = "Unauthorized";
                    break;
            }

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };

            context.ExceptionHandled = true;

            Console.WriteLine($"✅ Exception handled by GlobalExceptionFilter: {context.Exception.GetType().Name}");
        }
    }
}