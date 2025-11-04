using LoggingDemo.Services;
using NLog;

namespace LoggingDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddControllersWithViews();

            // Register custom services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Get logger
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                logger.Debug("Configuring application pipeline...");

                // Configure the HTTP request pipeline.
                if (!env.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });

                logger.Info("Application configured successfully");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error configuring application");
                throw;
            }
        }
    }
}