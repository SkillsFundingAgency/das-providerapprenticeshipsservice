using SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed
{
    public class ContractFeedProcessorHttpClient : IContractFeedProcessorHttpClient
    {
        private readonly AzureAuthentication _authenticationCredentials;
        private static readonly HttpClient _httpClient;
        private static string VendorAtomMediaType = "application/vnd.sfa.contract.v1+atom+xml";

        static ContractFeedProcessorHttpClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(VendorAtomMediaType));
        }
        
        public ContractFeedProcessorHttpClient(IContractFeedConfiguration config)
        {
            _authenticationCredentials = new AzureAuthentication(config.AADInstance, config.Tenant, config.ClientId, config.AppKey, config.ResourceId);
            BaseAddress = config.BaseAddress;
        }

        public string BaseAddress { get; }

        public virtual HttpClient GetAuthorizedHttpClient()
        {
            var token = _authenticationCredentials.GetAuthenticationResult().Result.AccessToken;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return _httpClient;
        }
    }
}