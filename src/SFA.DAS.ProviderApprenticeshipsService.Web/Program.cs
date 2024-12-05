using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;

namespace SFA.DAS.ProviderApprenticeshipsService.Web;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseNServiceBusContainer()
            .ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Information))
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}