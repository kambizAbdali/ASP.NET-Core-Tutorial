using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPageDemo.Filters
{
    public class ValidationPageFilter : IAsyncPageFilter
    {
        private readonly ILogger<ValidationPageFilter> _logger;

        public ValidationPageFilter(ILogger<ValidationPageFilter> logger)
        {
            _logger = logger;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HandlerMethod?.Name?.StartsWith("OnPost") == true)
            {
                _logger.LogInformation("Validating model for {HandlerName}", context.HandlerMethod.Name);

                if (!context.ModelState.IsValid)
                {
                    var errors = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    _logger.LogWarning("Model validation failed: {Errors}", string.Join(", ", errors));
                }
            }

            await next();
        }
    }
}