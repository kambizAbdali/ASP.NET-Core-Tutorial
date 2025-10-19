using RazorPageDemo.Filters;
using RazorPageDemo.Services;
using RazorPageDemo.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// افزودن Entity Framework In-Memory Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("RazorPageDemoDb"));

// افزودن سرویس‌ها
builder.Services.AddRazorPages();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<LoggingPageFilter>();
builder.Services.AddScoped<ValidationPageFilter>();

// پیکربندی Razor Pages با فیلترها
builder.Services.AddRazorPages().AddMvcOptions(options =>
{
   // options.Conventions.AddPageRoute("/Index", "");
    options.Filters.AddService<LoggingPageFilter>();
    options.Filters.AddService<ValidationPageFilter>();
});

// پیکربندی lowercase URLs
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

var app = builder.Build();

// مقداردهی اولیه دیتابیس
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated(); // این خط داده‌های اولیه را ایجاد می‌کند
}

// پیکربندی pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();