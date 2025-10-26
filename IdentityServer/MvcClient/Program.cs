// Program.cs
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// پیکربندی احراز هویت
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "mvc_client_cookie";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = "https://localhost:5001";  // آدرس Identity Server
    options.ClientId = "mvc_client";  // شناسه کلاینت
    options.ClientSecret = "mvc_secret";  // رمز کلاینت

    options.ResponseType = "code";  // Authorization Code Flow
    options.UsePkce = true;  // استفاده از PKCE

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("roles");
    options.Scope.Add("weather_api.read");
    options.Scope.Add("user_api.read");
    options.Scope.Add("offline_access");  // برای Refresh Token

    options.SaveTokens = true;  // ذخیره توکن‌ها
    options.GetClaimsFromUserInfoEndpoint = true;  // دریافت claimها از UserInfo endpoint

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };

    // events برای لاگ کردن
    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("role", "admin"));
});

builder.Services.AddHttpClient();  // برای فراخوانی API
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();  // فعال‌سازی احراز هویت
app.UseAuthorization();   // فعال‌سازی مجوزدهی

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();