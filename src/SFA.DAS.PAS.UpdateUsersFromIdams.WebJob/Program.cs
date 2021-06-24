using System;
using System.Diagnostics;
using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.DependencyResolution;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var container = IoC.Initialize();
                var logger = container.GetInstance<ILog>();
                logger.Info("UpdateUsersFromIdams job started");
                var timer = Stopwatch.StartNew();

                var service = container.GetInstance<IIdamsSyncService>();

                service.SyncUsers().Wait();
                timer.Stop();

                logger.Info($"UpdateUsersFromIdams job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (AggregateException exc)
            {
                ILog exLogger = new NLogLogger();
                exLogger.Error(exc, "Error running UpdateUsersFromIdams WebJob");
                exc.Handle(ex =>
                {
                    exLogger.Error(ex, "Inner exception running UpdateUsersFromIdams WebJob");
                    return false;
                });
            }
            catch (Exception ex)
            {
                ILog exLogger = new NLogLogger();
                exLogger.Error(ex, "Error running UpdateUsersFromIdams WebJob");
                throw;
            }
        }
    }
}
