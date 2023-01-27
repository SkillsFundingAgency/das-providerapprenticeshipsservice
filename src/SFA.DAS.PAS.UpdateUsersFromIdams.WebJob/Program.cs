using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob;

public class Program
{
    public static void Main(string[] args)
    {
        static void Main(string[] args)
        {
            try
            {
                var container = new ServiceCollection();
                var provider = container.BuildServiceProvider();
                var logger = provider.GetService<ILog>();
                
                logger.Info("UpdateUsersFromIdams job started");
                var timer = Stopwatch.StartNew();

                var service = provider.GetService<IIdamsSyncService>();

                service.SyncUsers().Wait();
                timer.Stop();

                logger.Info($"UpdateUsersFromIdams job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (AggregateException exc)
            {
                webBuilder
                    .UseStartup<Startup>()
                    .UseNLog();
            });
}