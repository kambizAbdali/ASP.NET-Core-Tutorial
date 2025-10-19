using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPageDemo.Filters
{
    public class LoggingPageFilter : IPageFilter
    {
        private readonly ILogger<LoggingPageFilter> _logger;

        public LoggingPageFilter(ILogger<LoggingPageFilter> logger)
        {
            _logger = logger;
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            _logger.LogInformation(
                "Page Selected: {PageName}, Handler: {HandlerName}",
                context.HandlerInstance.GetType().Name,
                context.HandlerMethod?.Name
            );
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            _logger.LogInformation(
                "Executing: {HandlerName}, Method: {Method}",
                context.HandlerMethod?.Name,
                context.HttpContext.Request.Method
            );
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            _logger.LogInformation(
                "Handler Executed: {HandlerName}, Success: {Success}",
                context.HandlerMethod?.Name,
                context.Exception == null
            );
        }
    }
}