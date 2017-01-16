using System;
using System.Diagnostics;
using System.Threading.Tasks;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.Data;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.DependencyResolution;

using ContractFeedConfiguration = SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.Configuration.ContractFeedConfiguration;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            // The following code ensures that the WebJob will be running continuously
            //var host = new JobHost();
            //host.RunAndBlock();

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
