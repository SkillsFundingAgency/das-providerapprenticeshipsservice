using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.DependencyResolution;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var container = IoC.Initialize();
                var logger = container.GetInstance<ILog>();
                logger.Info("Update IDAMS Users Provider job started");
                var timer = Stopwatch.StartNew();

                //var service = container.GetInstance<ImportProviderService>();

                //await service.Import();
                timer.Stop();

                logger.Info($"UpdateUsersFromIdams For Provider {0} job done, Took: {timer.ElapsedMilliseconds} milliseconds");
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
