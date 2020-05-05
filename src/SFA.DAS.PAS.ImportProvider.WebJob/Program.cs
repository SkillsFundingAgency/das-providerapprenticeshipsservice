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
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            try
            {
                var container = IoC.Initialize();
                var logger = container.GetInstance<ILog>();
                logger.Info("ContractAgreements job started");
                var timer = Stopwatch.StartNew();

                var service = container.GetInstance<ImportProviderService>();

                service.Import().Wait();
                timer.Stop();

                logger.Info($"ContractAgreements job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (Exception ex)
            {
                ILog exLogger = new NLogLogger();
                exLogger.Error(ex, "Error running ContractAgreements WebJob");
                throw;
            }
        }
    }
}
