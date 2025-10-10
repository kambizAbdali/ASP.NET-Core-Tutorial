using Microsoft.AspNetCore.Mvc.Filters;

namespace FilterDemo.Filters.ResultFilter
{
    public class AddHeaderResultFilter :Attribute, IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Before result execution - Add custom headers only if they don't exist
            // قبل از اجرای نتیجه - اضافه کردن هدرهای سفارشی فقط در صورت عدم وجود
            var response = context.HttpContext.Response;

            // Safe header addition - check if header exists first
            // اضافه کردن ایمن هدر - ابتدا بررسی وجود هدر
            SafeAddHeader(response, "X-Application-Name", "Filter Demo API");
            SafeAddHeader(response, "X-Application-Version", "1.0.0");
            SafeAddHeader(response, "X-Response-Time", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            SafeAddHeader(response, "X-Developer", "ASP.NET Core Team");
            SafeAddHeader(response, "X-Filter-Applied", "AddHeaderResultFilter");

            Console.WriteLine("📨 Custom headers safely added to response");

            Console.WriteLine("📨 Custom headers added to response");
        }
        private void SafeAddHeader(HttpResponse response, string headerName, string headerValue)
        {
            // Check if header already exists
            // بررسی آیا هدر از قبل وجود دارد
            if (!response.Headers.ContainsKey(headerName))
            {
                response.Headers.Add(headerName, headerValue);
                Console.WriteLine($"   ➕ Added header: {headerName} = {headerValue}");
            }
            else
            {
                Console.WriteLine($"   ⚠️ Header already exists: {headerName}");
            }
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {
            // After result execution - Log completion
            // بعد از اجرای نتیجه - ثبت تکمیل
            Console.WriteLine($"✅ Response sent with status code: {context.HttpContext.Response.StatusCode}");
        }
    }
}