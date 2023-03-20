using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    public interface IPasAccountApiConfiguration : IBaseConfiguration
    {
        bool CheckForContractAgreements { get; set; }
        string ContractAgreementsUrl { get; set; }
    }

    public class PasAccountApiConfiguration : IPasAccountApiConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public bool CheckForContractAgreements { get; set; }
        public string ContractAgreementsUrl { get; set; }
        public PasAccountApiClientConfiguration PasAccountApiClientConfiguration { get; set; }
    }
}
