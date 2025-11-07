using Microsoft.EntityFrameworkCore;
using SignalRChatApp.Data;
using SignalRChatApp.Hubs;
using SignalRChatApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add SignalR
builder.Services.AddSignalR();

// Add Entity Framework with retry on failure
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    ));

// Configure Identity
builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.User.RequireUniqueEmail = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager<SignInManager<IdentityUser>>()
.AddDefaultTokenProviders();

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies(options => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OperatorOnly", policy =>
        policy.RequireRole("Operator"));
});

// Add custom services
builder.Services.AddScoped<IChatRoomService, ChatRoomService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IConnectionManager, ConnectionManager>();


// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Use CORS before other middleware
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map SignalR Hubs
app.MapHub<ChatHub>("/chathub");
app.MapHub<SupportHub>("/supporthub");

// Ensure database is created and seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        // Get the database context
        var context = services.GetRequiredService<ApplicationDbContext>();

        // This will create the database if it doesn't exist and apply any pending migrations
        await context.Database.EnsureCreatedAsync();

        // Alternative: You can also use Migrations (recommended for production)
        // await context.Database.MigrateAsync();

        Console.WriteLine("Database created successfully!");

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Create Operator role
        if (!await roleManager.RoleExistsAsync("Operator"))
        {
            await roleManager.CreateAsync(new IdentityRole("Operator"));
            Console.WriteLine("Operator role created successfully.");
        }

        // Create default operator user
        var operatorUser = await userManager.FindByNameAsync("operator");
        if (operatorUser == null)
        {
            operatorUser = new IdentityUser { UserName = "operator", Email = "operator@example.com" };
            var result = await userManager.CreateAsync(operatorUser, "operator123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(operatorUser, "Operator");
                Console.WriteLine("Operator user created successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to create operator user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while creating the database: {ex.Message}");
        // Don't rethrow the exception to allow the app to start
        // You might want to handle this differently in production
    }
}

app.Run();