using NLog.Web;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;

namespace SFA.DAS.ProviderApprenticeshipsService.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var logger = NLogBuilder.ConfigureNLog(environment == "Development" ? "nlog.Development.config" : "nlog.config").GetCurrentClassLogger();
        logger.Info("Starting up host");

        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseNLog()
            .UseNServiceBusContainer()
            .ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Information))
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}