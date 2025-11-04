using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Interfaces.IServices;
using TaskManagement.Core.Services;
using TaskManagement.Infrastructure;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add API Versioning
builder.Services.AddApiVersioning(options => // Configures API versioning services.
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // Sets the default API version to 1.0.
    options.AssumeDefaultVersionWhenUnspecified = true; // If no version is specified, assume the default.
    options.ReportApiVersions = true; // Includes the supported API versions in the response headers.
    options.ApiVersionReader = ApiVersionReader.Combine( // Defines how the API version is read from the request.
        new UrlSegmentApiVersionReader(), // Reads the version from the URL segment (e.g., /v1/controller).
        new HeaderApiVersionReader("x-api-version"), // Reads the version from the "x-api-version" header.
        new QueryStringApiVersionReader("api-version")  // Reads the version from the "api-version" query string parameter.
    );
});

// Add API Explorer for versioning
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Sets the format for the API version group name (e.g., 'v1').
    options.SubstituteApiVersionInUrl = true;
});
// Add Infrastructure layer (EF Core, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Core services
builder.Services.AddScoped<ITaskService, TaskService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Sets the default authentication scheme to JWT Bearer.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Sets the default challenge scheme to JWT Bearer.
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // برای توسعه غیرفعال کنید
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters // Configures the token validation parameters.
    {
        ValidateIssuerSigningKey = true, // Specifies that the issuer's signing key should be validated.
        IssuerSigningKey = new SymmetricSecurityKey(key), // Sets the issuer's signing key.
        ValidateIssuer = true, // Specifies that the issuer should be validated.
        ValidIssuer = jwtSettings["Issuer"], // Sets the valid issuer from the JWT settings.
        ValidateAudience = true, // Specifies that the audience should be validated.
        ValidAudience = jwtSettings["Audience"], // Sets the valid audience from the JWT settings.
        ValidateLifetime = true, // Specifies that the token's lifetime should be validated.
        ClockSkew = TimeSpan.Zero // Sets the clock skew to zero, meaning no tolerance for time differences.
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

// Add Authorization
builder.Services.AddAuthorization();

// Add HttpContextAccessor for accessing current user in services
builder.Services.AddHttpContextAccessor();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Management API",
        Version = "v1",
        Description = "Version 1.0 (Deprecated)",
        Contact = new OpenApiContact { Name = "API Support", Email = "support@taskmanagement.com" }
    });

    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Task Management API",
        Version = "v2",
        Description = "Version 2.0 with enhanced features",
        Contact = new OpenApiContact { Name = "API Support", Email = "support@taskmanagement.com" }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Include XML comments for API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskManagementContext>();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Task Management API v2");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    if (endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null)
    {
        Console.WriteLine($"Authorize attribute found on: {context.Request.Path}");

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        Console.WriteLine($"Authorization Header: {authHeader}");

        if (string.IsNullOrEmpty(authHeader))
        {
            Console.WriteLine("No Authorization header found");
        }
        else
        {
            Console.WriteLine($"User Authenticated: {context.User?.Identity?.IsAuthenticated}");
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                foreach (var claim in context.User.Claims)
                {
                    Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                }
            }
        }
    }
    await next();
});

// Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();