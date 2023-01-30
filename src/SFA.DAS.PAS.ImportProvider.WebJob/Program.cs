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
            try
            {
                var container = new ServiceCollection();
                var provider = container.BuildServiceProvider();
                var logger = provider.GetService<ILogger<Program>>();

                logger.LogInformation("ImportProvider job started");
                var timer = Stopwatch.StartNew();

                var service = provider.GetService<IImportProvider>();

                service.Import();
                timer.Stop();

                logger.LogInformation($"ImportProvider job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (AggregateException exc)
            {
                ILoggerFactory loggerFactory = new LoggerFactory();
                ILogger exLogger = loggerFactory.CreateLogger<Program>();
                exLogger.LogError(exc, "Error running ImportProvider WebJob");
                exc.Handle(ex =>
                {
                    exLogger.LogError(ex, "Inner exception running ImportProvider WebJob");
                    return false;
                });
            }
            catch (Exception ex)
            {
                ILoggerFactory loggerFactory = new LoggerFactory();
                ILogger exLogger = loggerFactory.CreateLogger<Program>();
                exLogger.LogError(ex, "Error running ImportProvider WebJob");
                throw;
            };
        }
    }
}
