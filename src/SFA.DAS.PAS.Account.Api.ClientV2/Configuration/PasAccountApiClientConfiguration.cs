using SFA.DAS.Http.Configuration;

namespace SFA.DAS.PAS.Account.Api.ClientV2.Configuration
{
    public class PasAccountApiClientConfiguration : IAzureActiveDirectoryClientConfiguration, IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }
}
