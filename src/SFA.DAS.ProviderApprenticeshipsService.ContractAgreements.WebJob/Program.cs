using System;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.DependencyResolution;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

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
                var config = container.GetInstance<IContractFeedConfiguration>();
                var logger = container.GetInstance<ILog>();

                logger.Info("ContractAgreements job started");
                var httpClient = new ContractFeedProcessorHttpClient(config);
                var reader = new ContractFeedReader(httpClient);

                var dataProvider = new ContractFeedProcessor(reader, new ContractFeedEventValidator(), logger);
                var repository = new ProviderAgreementStatusRepository(logger);

                var service = new ProviderAgreementStatusService(dataProvider, repository, logger);

                service.UpdateProviderAgreementStatuses();

                logger.Info("ContractAgreements job done");
            }

            catch (Exception ex)
            {
                ILog exLogger = new NLogLogger();
                exLogger.Error(ex, "Error running ContractAgreements WebJob");
            }
        }
    }
}
