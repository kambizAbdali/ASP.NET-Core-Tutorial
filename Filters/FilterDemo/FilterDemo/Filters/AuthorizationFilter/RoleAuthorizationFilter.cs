using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FilterDemo.Filters.AuthorizationFilter
{
    public class RoleAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string _requiredRole;

        public RoleAuthorizationFilter(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get user role from header (in real app, from token)
            // دریافت نقش کاربر از هدر (در برنامه واقعی از توکن)
            var userRole = context.HttpContext.Request.Headers["User-Role"].ToString();

            if (string.IsNullOrEmpty(userRole))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    Error = "Role required",
                    Message = "User-Role header is missing"
                });
                return;
            }

            // Check if user has required role
            // بررسی آیا کاربر نقش مورد نیاز را دارد
            if (userRole != _requiredRole)
            {
                context.Result = new ForbidResult();
            }
        }
    }

    // Attribute for role-based authorization
    // اتریبیوت برای احراز هویت مبتنی بر نقش
    public class RequireRoleAttribute : TypeFilterAttribute
    {
        public RequireRoleAttribute(string role) : base(typeof(RoleAuthorizationFilter))
        {
            Arguments = new object[] { role };
        }
    }
}