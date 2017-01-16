using System.Net.Http;
using System.Net.Http.Headers;

using ContractFeedConfiguration = SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.Configuration.ContractFeedConfiguration;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed
{
    public class ContractFeedProcessorHttpClient : IContractFeedProcessorHttpClient
    {
        private readonly AzureAuthentication _authenticationCredentials;

        public ContractFeedProcessorHttpClient(ContractFeedConfiguration config)
        {
            _authenticationCredentials = new AzureAuthentication(config.AADInstance, config.Tenant, config.ClientId, config.AppKey, config.ResourceId);
            BaseAddress = config.BaseAddress;
        }

        public string BaseAddress { get; }

        public virtual HttpClient GetAuthorizedHttpClient()
        {
            var client = new HttpClient();
            var token = _authenticationCredentials.GetAuthenticationResult().Result.AccessToken;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
    }
}