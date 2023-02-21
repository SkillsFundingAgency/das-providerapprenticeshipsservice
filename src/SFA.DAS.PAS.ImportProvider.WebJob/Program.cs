using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System.Configuration;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using System.Net.Http;
using Microsoft.Extensions.Hosting;
using SFA.DAS.PAS.ImportProvider.WebJob.Extensions;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.ImportProvider.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = CreateHost())
            {
                await ImportProvider(host);

                await host.RunAsync();
            }
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
            };
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
