using FilterDemo.Filters.ActionFilter;
using FilterDemo.Filters.AuthorizationFilter;
using FilterDemo.Filters.ExceptionFilter;
using FilterDemo.Filters.ResourceFilter;
using FilterDemo.Filters.ResultFilter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Register filter services for dependency injection
// ثبت سرویس‌های فیلتر برای تزریق وابستگی
builder.Services.AddScoped<ApiKeyAuthorizationFilter>();
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<ValidateModelFilter>();
builder.Services.AddScoped<GlobalExceptionFilter>();
builder.Services.AddScoped<CustomExceptionFilter>();
builder.Services.AddScoped<AddHeaderResultFilter>();
builder.Services.AddScoped<FormatResponseFilter>();
builder.Services.AddScoped<CacheResourceFilter>();
builder.Services.AddScoped<TimingResourceFilter>();

// Add global filters
// اضافه کردن فیلترهای سراسری
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); // Global exception handling
    options.Filters.Add<AddHeaderResultFilter>(); // Add headers to all responses
    options.Filters.Add<LogActionFilter>(); // Log all actions
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine("🚀 Filter Demo Application Started");
Console.WriteLine("📝 Check the console for filter execution logs");
Console.WriteLine("🔧 Test the endpoints with appropriate headers");

app.Run();