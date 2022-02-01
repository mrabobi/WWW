using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace SmartHome.API
{
    public static class ServiceExtensions
    {
        public static void ConfigureLogging(this IServiceCollection service, IConfiguration configuration)
        {
            var logFilePath = configuration.GetValue("LogFilePath", "Logs.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(logFilePath)
                .CreateLogger();
        }
    }

}
