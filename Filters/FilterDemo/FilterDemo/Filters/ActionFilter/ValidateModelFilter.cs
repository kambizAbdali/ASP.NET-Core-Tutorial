using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilterDemo.Filters.ActionFilter
{
    public class ValidateModelFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Check model state before action execution
            // بررسی وضعیت مدل قبل از اجرای اکشن
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                context.Result = new BadRequestObjectResult(new
                {
                    Error = "Validation failed",
                    Message = "Please check the input data",
                    Details = errors
                });

                Console.WriteLine("❌ Model validation failed");
            }
            else
            {
                Console.WriteLine("✅ Model validation passed");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Can be used for post-action processing
            // قابل استفاده برای پردازش پس از اکشن
        }
    }

    // Attribute for model validation
    // اتریبیوت برای اعتبارسنجی مدل
    public class ValidateModelAttribute : TypeFilterAttribute
    {
        public ValidateModelAttribute() : base(typeof(ValidateModelFilter))
        {
        }
    }
}