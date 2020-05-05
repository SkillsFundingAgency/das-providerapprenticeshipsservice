using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System;


namespace SFA.DAS.PAS.ImportProvider.WebJob.Configuration
{
    public class ImportProviderConfiguration : IImportProviderConfiguration
    {
        public ApprenticeshipInfoServiceConfiguration ApprenticeshipInfoService { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string DatabaseConnectionString { get; set; }
    }
}
