using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace FilterDemo.Filters.ResourceFilter
{
    public class TimingResourceFilter : IResourceFilter
    {
        private Stopwatch _stopwatch;
        private readonly ILogger<TimingResourceFilter> _logger;

        public TimingResourceFilter(ILogger<TimingResourceFilter> logger)
        {
            _logger = logger;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // Start timing before resource execution
            // شروع زمان‌سنجی قبل از اجرای منبع
            _stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("🕐 Starting resource execution timing");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // Stop timing after resource execution
            // توقف زمان‌سنجی بعد از اجرای منبع
            _stopwatch.Stop();
            var elapsedMs = _stopwatch.ElapsedMilliseconds;

            _logger.LogInformation($"⏰ Resource execution completed in {elapsedMs}ms");

            // Warning: Long execution time
            // هشدار: زمان اجرای طولانی
            if (elapsedMs > 1000)
            {
                _logger.LogWarning($"🚨 Long execution time detected: {elapsedMs}ms");
            }
        }
    }
}