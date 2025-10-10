using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilterDemo.Filters.ResourceFilter
{
    public class CacheResourceFilter : IResourceFilter
    {
        private static readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
        private static readonly object _lockObject = new object();
        private string _cacheKey;

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // Before action execution - Check cache
            // قبل از اجرای اکشن - بررسی کش
            _cacheKey = $"cache_{context.HttpContext.Request.Method}_{context.HttpContext.Request.Path}";

            lock (_lockObject)
            {
                if (_cache.ContainsKey(_cacheKey))
                {
                    // Return cached result
                    // بازگرداندن نتیجه از کش
                    context.Result = (IActionResult)_cache[_cacheKey];
                    Console.WriteLine($"✅ Returning cached result for {_cacheKey}");
                }
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // After action execution - Store in cache
            // بعد از اجرای اکشن - ذخیره در کش
            if (!string.IsNullOrEmpty(_cacheKey) && context.Result is OkObjectResult)
            {
                lock (_lockObject)
                {
                    _cache[_cacheKey] = context.Result;
                    Console.WriteLine($"💾 Stored in cache: {_cacheKey}");
                }
            }
        }
    }
}