using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var container = new ServiceCollection();
            var provider = container.BuildServiceProvider();
            var logger = provider.GetService<ILogger<Program>>();

            logger.LogInformation("UpdateUsersFromIdams job started");
            var timer = Stopwatch.StartNew();

            var service = provider.GetService<IIdamsSyncService>();

            service.SyncUsers().Wait();
            timer.Stop();

            logger.LogInformation($"UpdateUsersFromIdams job done, Took: {timer.ElapsedMilliseconds} milliseconds");
        }
        catch (AggregateException exc)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger exLogger = loggerFactory.CreateLogger<Program>();
            exLogger.LogError(exc, "Error running UpdateUsersFromIdams WebJob");
            exc.Handle(ex =>
            {
                exLogger.LogError(ex, "Inner exception running UpdateUsersFromIdams WebJob");
                return false;
            });
        }
        catch (Exception ex)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger exLogger = loggerFactory.CreateLogger<Program>();
            exLogger.LogError(ex, "Error running UpdateUsersFromIdams WebJob");
            throw;
        };
    } 
}