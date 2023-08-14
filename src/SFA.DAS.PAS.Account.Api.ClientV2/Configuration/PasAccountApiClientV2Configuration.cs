using SFA.DAS.Http.Configuration;

namespace SFA.DAS.PAS.Account.Api.ClientV2.Configuration
{
    public class PasAccountApiClientV2Configuration : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string IdentifierUri { get; set; }
    }
}
