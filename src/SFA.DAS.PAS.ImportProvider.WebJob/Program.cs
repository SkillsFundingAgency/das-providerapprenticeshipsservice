using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;
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

        await ImportProvider(host);

        await host.RunAsync();
    }
    private static async Task ImportProvider(IHost host)
    {
        ILoggerFactory loggerFactory = new LoggerFactory();
        ILogger logger = loggerFactory.CreateLogger<Program>();

        try
        {
            logger.LogInformation("ImportProvider job started");
            var timer = Stopwatch.StartNew();

            var importService = host.Services.GetService<IImportProviderService>();
            await importService.Import();

            timer.Stop();
            logger.LogInformation($"ImportProvider job done, Took: {timer.ElapsedMilliseconds} milliseconds");
        }
        catch (AggregateException exc)
        {
            logger.LogError(exc, "Error running ImportProvider WebJob");
            exc.Handle(ex =>
            {
                logger.LogError(ex, "Inner exception running ImportProvider WebJob");
                return false;
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error running ImportProvider WebJob");
            throw;
        }
    }

    private static IHost CreateHost()
    {
        var builder = new HostBuilder()
            .UseDasEnvironment()
            .AddConfiguration()
            .ConfigureServices();

        return builder.Build();
    }
}