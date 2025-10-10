using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilterDemo.Filters.ActionFilter
{
    public class LogActionFilter : Attribute, IActionFilter
    {
        private readonly ILogger<LogActionFilter> _logger;

        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Before action execution
            // قبل از اجرای اکشن
            _logger.LogInformation($"🚀 Starting action: {context.ActionDescriptor.DisplayName}");
            _logger.LogInformation($"📝 Route data: {string.Join(", ", context.RouteData.Values)}");

            // Log action parameters
            // ثبت پارامترهای اکشن
            foreach (var argument in context.ActionArguments)
            {
                _logger.LogInformation($"📦 Parameter: {argument.Key} = {argument.Value}");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // After action execution
            // بعد از اجرای اکشن
            if (context.Exception != null)
            {
                _logger.LogError(context.Exception, $"❌ Action failed: {context.ActionDescriptor.DisplayName}");
            }
            else
            {
                _logger.LogInformation($"✅ Action completed: {context.ActionDescriptor.DisplayName}");

                if (context.Result is ObjectResult objectResult)
                {
                    _logger.LogInformation($"📊 Result type: {objectResult.Value?.GetType().Name}");
                }
            }
        }
    }
}