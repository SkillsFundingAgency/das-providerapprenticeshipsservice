using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.PAS.ImportProvider.WebJob.Extensions;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.ImportProvider.WebJob;

class Program
{
    public static async Task Main()
    {
        using var host = CreateHost();

        var logger = host.Services.GetService<ILogger<Program>>();
        logger.LogInformation("SFA.DAS.PAS.ImportProvider.WebJob starting up ...");

        await host.RunAsync();
    }

    private static IHost CreateHost()
    {
        return new HostBuilder()
            .UseDasEnvironment()
            .AddConfiguration()
            .ConfigureDasLogging()
            .ConfigureServices()
            .ConfigureDasWebJobs()
            .Build();
    }
}