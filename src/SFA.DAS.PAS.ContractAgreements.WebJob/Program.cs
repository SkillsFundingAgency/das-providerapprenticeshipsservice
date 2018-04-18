using System;
using System.Diagnostics;
using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.ContractAgreements.WebJob.DependencyResolution;

namespace SFA.DAS.PAS.ContractAgreements.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        static void Main()
        {
            try
            {
                var container = IoC.Initialize();
                var logger = container.GetInstance<ILog>();
                logger.Info("ContractAgreements job started");
                var timer = Stopwatch.StartNew();

                var service = container.GetInstance<ProviderAgreementStatusService>();
                service.UpdateProviderAgreementStatuses().Wait();

                timer.Stop();

                logger.Info($"ContractAgreements job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (Exception ex)
            {
                ILog exLogger = new NLogLogger();
                exLogger.Error(ex, "Error running ContractAgreements WebJob");
            }
        }
    }
}
