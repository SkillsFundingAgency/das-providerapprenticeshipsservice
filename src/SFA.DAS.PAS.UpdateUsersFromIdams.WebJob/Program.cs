using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Extensions;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob;

public class Program
{
    public static async Task Main()
    {
        using (var host = CreateHost())
        {
            await SyncIdamsUsers(host);

            await host.RunAsync();
        }
    }
    private static async Task SyncIdamsUsers(IHost host)
    {
        ILoggerFactory loggerFactory = new LoggerFactory();
        ILogger logger = loggerFactory.CreateLogger<Program>();

        try
        {
            logger.LogInformation("UpdateUsersFromIdams job started");
            var timer = Stopwatch.StartNew();

            var service = host.Services.GetService<IIdamsSyncService>();
            await service.SyncUsers();

            timer.Stop();

            logger.LogInformation($"UpdateUsersFromIdams job done, Took: {timer.ElapsedMilliseconds} milliseconds");
        }
        catch (AggregateException exc)
        {
            logger.LogError(exc, "Error running UpdateUsersFromIdams WebJob");
            exc.Handle(ex =>
            {
                logger.LogError(ex, "Inner exception running UpdateUsersFromIdams WebJob");
                return false;
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error running UpdateUsersFromIdams WebJob");
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