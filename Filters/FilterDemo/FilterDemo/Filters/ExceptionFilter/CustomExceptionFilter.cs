using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilterDemo.Filters.ExceptionFilter
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // Handle specific business exceptions
            // مدیریت خطاهای خاص کسب‌وکار
            if (context.Exception is InvalidOperationException invalidOpEx)
            {
                _logger.LogWarning(invalidOpEx, "Business rule violation");

                context.Result = new ObjectResult(new
                {
                    Error = "Business Rule Violation",
                    Message = invalidOpEx.Message,
                    Suggestion = "Please check your input and try again"
                })
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };

                context.ExceptionHandled = true;
                Console.WriteLine("✅ Business exception handled by CustomExceptionFilter");
            }
        }
    }
}