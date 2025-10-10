using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilterDemo.Filters.AuthorizationFilter
{
    public class ApiKeyAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if API Key exists in header
            // بررسی وجود کلید API در هدر
            if (!context.HttpContext.Request.Headers.ContainsKey("X-API-Key"))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    Error = "API Key is required",
                    Message = "Please provide X-API-Key in header"
                });
                return;
            }

            var apiKey = context.HttpContext.Request.Headers["X-API-Key"].ToString();

            // Simple API Key validation
            // اعتبارسنجی ساده کلید API
            if (apiKey != "12345-ABCDE-67890")
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    Error = "Invalid API Key",
                    Message = "The provided API key is not valid"
                });
            }
        }
    }

    // Attribute for easy use
    // اتریبیوت برای استفاده آسان
    public class RequireApiKeyAttribute : TypeFilterAttribute
    {
        public RequireApiKeyAttribute() : base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}