using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.ContractAgreements.WebJob.Extensions;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;

namespace SFA.DAS.PAS.ContractAgreements.WebJob;

public class Program
{
    public static async Task Main()
    {
        using var host = CreateHost();
        await UpdateProviderAgreementStatuses(host);

        await host.RunAsync();
    }

    private static async Task UpdateProviderAgreementStatuses(IHost host)
    {
        ILoggerFactory loggerFactory = new LoggerFactory();
        ILogger logger = loggerFactory.CreateLogger<Program>();

        try
        {
            logger.LogInformation("ContractAgreements job started");

            var timer = Stopwatch.StartNew();

            var providerAgreementStatusService = host.Services.GetService<IProviderAgreementStatusService>();
            await providerAgreementStatusService.UpdateProviderAgreementStatuses();

            timer.Stop();

            logger.LogInformation($"ContractAgreements job done, Took: {timer.ElapsedMilliseconds} milliseconds");
        }
        catch (AggregateException exc)
        {
            logger.LogError(exc, "Error running ContractAgreements WebJob");
            exc.Handle(ex =>
            {
                logger.LogError(ex, "Inner exception running ContractAgreements WebJob");
                return false;
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error running ContractAgreements WebJob");
            throw;
        }
    }

    private static IHost CreateHost()
    {
        return new HostBuilder()
            .UseDasEnvironment()
            .AddConfiguration()
            .ConfigureDasLogging()
            .ConfigureServices()
            .Build();
    }
}