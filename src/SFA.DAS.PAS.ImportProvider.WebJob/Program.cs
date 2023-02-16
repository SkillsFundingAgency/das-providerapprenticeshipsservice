using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.PAS.ImportProvider.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        public static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger logger = loggerFactory.CreateLogger<Program>();

            try
            {
                var services = new ServiceCollection();
                services.AddTransient<IImportProviderService, ImportProviderService>();
                var provider = services.BuildServiceProvider();

                logger.LogInformation("ImportProvider job started");
                var timer = Stopwatch.StartNew();

                var importService = provider.GetService<IImportProviderService>();

                importService.Import();
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
    }
}
