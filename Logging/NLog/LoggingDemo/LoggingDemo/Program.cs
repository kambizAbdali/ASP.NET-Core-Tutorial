using NLog.Web;

namespace LoggingDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create logger for startup process
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("Application starting up...");

                var builder = WebApplication.CreateBuilder(args);

                // Configure logging - Clear default providers and use NLog
                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                // Use Startup class for configuration
                var startup = new Startup(builder.Configuration);
                startup.ConfigureServices(builder.Services);

                var app = builder.Build();

                startup.Configure(app, app.Environment);

                logger.Info("Application started successfully");
                app.Run();
            }
            catch (Exception ex)
            {
                // Catch setup errors
                logger.Error(ex, "Application failed to start due to an exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit
                NLog.LogManager.Shutdown();
            }
        }
    }
}