using ModelBindingDemo.Models.Data;
using ModelBindingDemo.ModelBinders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
    });

// Register Custom Model Binder
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new CsvArrayModelBinderProvider());
    options.ModelBinderProviders.Insert(1, new CustomDateModelBinderProvider());
});

// Support for XML
builder.Services.AddControllers()
    .AddXmlSerializerFormatters();

// Add In-Memory Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ModelBindingDemoDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.Run();