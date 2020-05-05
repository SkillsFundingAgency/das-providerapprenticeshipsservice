using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System;


namespace SFA.DAS.PAS.ImportProvider.WebJob.Configuration
{
    public class ImportProviderConfiguration : IImportProviderConfiguration
    {
        public string BaseUrl { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string DatabaseConnectionString { get; set; }
    }
}
