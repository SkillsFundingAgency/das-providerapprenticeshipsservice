using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    // ToDo: Do we need to implement IConfiguration?
    // ToDo: Do we need IContractFeedConfiguration?
    public class ContractFeedConfiguration : IConfiguration, IContractFeedConfiguration
    {
        public string AADInstance { get; set; }

        public string Tenant { get; set; }

        public string ClientId { get; set; }

        public string AppKey { get; set; }

        public string ResourceId { get; set; }

        public string BaseAddress { get; set; }

        // Needed?
        public string DatabaseConnectionString { get; set; }

        // Needed?
        public string ServiceBusConnectionString { get; set; }
    }

    public interface IContractFeedConfiguration
    {
         string AADInstance { get; set; }

        string Tenant { get; set; }

        string ClientId { get; set; }

        string AppKey { get; set; }

        string ResourceId { get; set; }

        string BaseAddress { get; set; }

        string DatabaseConnectionString { get; set; }

        string ServiceBusConnectionString { get; set; }
    }
}