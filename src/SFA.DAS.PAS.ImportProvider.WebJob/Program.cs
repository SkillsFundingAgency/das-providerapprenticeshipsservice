using System;
using System.Diagnostics;
using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.ImportProvider.WebJob.DependencyResolution;
using SFA.DAS.PAS.ImportProvider.WebJob.Importer;

namespace SFA.DAS.PAS.ImportProvider.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        static void Main()
        {
            try
            {
                var container = IoC.Initialize();
                var logger = container.GetInstance<ILog>();
                logger.Info("Import Provider job started");
                var timer = Stopwatch.StartNew();

                var service = container.GetInstance<ImportProviderService>();

                service.Import().Wait();
                timer.Stop();

                logger.Info($"ImportProvider job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (Exception ex)
            {
                ILog exLogger = new NLogLogger();
                exLogger.Error(ex, "Error running ImportProvider WebJob");
                throw;
            }
        }
    }
}
