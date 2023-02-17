// This application entry point is based on ASP.NET Core new project templates and is included
// as a starting point for app host configuration.
// This file may need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core hosting, see https://docs.microsoft.com/aspnet/core/fundamentals/host/web-host

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.ContractAgreements.WebJob.Extensions;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;

namespace SFA.DAS.PAS.ContractAgreements.WebJob
{
    public class Program
    {
        public static async Task Main()
        {
            using (var host = CreateHost())
            {
                await UpdateProviderAgreementStatuses(host);

                await host.RunAsync();
            }
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
            var builder = new HostBuilder()
                .AddConfiguration()
                .ConfigureServices();

            return builder.Build();
        }
    }
}
