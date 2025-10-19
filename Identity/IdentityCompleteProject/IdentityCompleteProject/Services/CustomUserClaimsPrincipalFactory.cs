using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using IdentityCompleteProject.Models;

namespace IdentityCompleteProject.Services
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>
    {
        public CustomUserClaimsPrincipalFactory(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            // Generate base claims (from default implementation)
            var identity = await base.GenerateClaimsAsync(user);

            // Add custom claims
            identity.AddClaim(new Claim("FirstName", user.FirstName ?? ""));
            identity.AddClaim(new Claim("LastName", user.LastName ?? ""));
            identity.AddClaim(new Claim("FullName", user.FullName));
            identity.AddClaim(new Claim("NationalCode", user.NationalCode ?? ""));

            // Calculate age and add as claim
            var age = DateTime.Now.Year - user.BirthDate.Year;
            if (user.BirthDate.Date > DateTime.Now.AddYears(-age)) age--;
            identity.AddClaim(new Claim("Age", age.ToString()));

            // Add purchase-related claims (business logic)
            identity.AddClaim(new Claim("MaxPurchaseCount", "10")); // Default value
            identity.AddClaim(new Claim("MonthlyPurchaseLimit", "1000000")); // 1,000,000 units

            // Add user roles as claims
            var roles = await UserManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
                identity.AddClaim(new Claim("Role", role)); // Additional role claim
            }

            // Add customer type based on registration date (business logic)
            var customerType = (DateTime.Now - user.CreatedDate).TotalDays > 365 ? "VIP" : "Regular";
            identity.AddClaim(new Claim("CustomerType", customerType));

            return identity;
        }
    }
}