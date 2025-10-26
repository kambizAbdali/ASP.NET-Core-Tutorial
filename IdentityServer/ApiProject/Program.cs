// Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// پیکربندی احراز هویت JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";  // آدرس Identity Server
        options.Audience = "weather_api";  // audience مورد انتظار
        options.RequireHttpsMetadata = false;  // فقط برای توسعه

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            }
        };
    });

// پیکربندی مجوزدهی
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("WeatherRead", policy =>
        policy.RequireClaim("scope", "weather_api.read"));

    options.AddPolicy("WeatherWrite", policy =>
        policy.RequireClaim("scope", "weather_api.write"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("role", "admin"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();  // فعال‌سازی احراز هویت
app.UseAuthorization();   // فعال‌سازی مجوزدهی

app.MapControllers();

app.Run();