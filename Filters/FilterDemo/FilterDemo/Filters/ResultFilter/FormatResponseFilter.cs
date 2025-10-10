using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilterDemo.Filters.ResultFilter
{
    public class FormatResponseFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Format the response before sending
            // فرمت‌دهی پاسخ قبل از ارسال
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var originalValue = objectResult.Value;
                var statusCode = objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;

                var formattedResponse = new
                {
                    Success = statusCode >= 200 && statusCode < 300,
                    Data = originalValue,
                    Timestamp = DateTime.UtcNow,
                    RequestId = context.HttpContext.TraceIdentifier
                };

                context.Result = new ObjectResult(formattedResponse)
                {
                    StatusCode = statusCode
                };

                Console.WriteLine("🔄 Response formatted with standard structure");
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Can be used for cleanup or final logging
            // قابل استفاده برای پاک‌سازی یا ثبت نهایی
        }
    }
}