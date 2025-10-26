using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// افزودن سرویس‌های لازم
builder.Services.AddControllersWithViews(); // برای پشتیبانی از MVC و Viewها
builder.Services.AddRazorPages();

// افزودن سرویس‌های Identity Server
builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryApiResources(Config.GetApiResources())
    .AddInMemoryApiScopes(Config.GetApiScopes())
    .AddInMemoryClients(Config.GetClients())
    .AddInMemoryIdentityResources(Config.GetIdentityResources())
    .AddTestUsers(Config.GetTestUsers());

var app = builder.Build();

// فعال‌سازی سرویس‌های استاتیک برای فایل‌های wwwroot
app.UseStaticFiles();

// فعال‌سازی routing
app.UseRouting();

// فعال‌سازی Identity Server
app.UseIdentityServer();

// فعال‌سازی authorization
app.UseAuthorization();

// پیکربندی endpoints
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();