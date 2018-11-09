using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Configuration
{
    public class AccountApiConfiguration : IAccountApiConfiguration, IConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
