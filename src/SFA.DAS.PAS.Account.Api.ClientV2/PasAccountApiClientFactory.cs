using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;

namespace SFA.DAS.PAS.Account.Api.ClientV2
{
    internal class PasAccountApiClientFactory
    {
        private readonly PasAccountApiClientV2Configuration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public PasAccountApiClientFactory(PasAccountApiClientV2Configuration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IPasAccountApiClient CreateClient()
        {
            IHttpClientFactory httpClientFactory;

            if (IsClientCredentialConfiguration(_configuration.ClientId, _configuration.ClientSecret, _configuration.Tenant))
            {
                httpClientFactory = new AzureActiveDirectoryHttpClientFactory(_configuration, _loggerFactory);
            }
            else
            {
                httpClientFactory = new ManagedIdentityHttpClientFactory(_configuration, _loggerFactory);
            }

            var httpClient = httpClientFactory.CreateHttpClient();

            var restHttpClient = new RestHttpClient(httpClient);
            var apiClient = new PasAccountApiClient(restHttpClient);

            return apiClient;
        }

        private bool IsClientCredentialConfiguration(string clientId, string clientSecret, string tenant)
        {
            return !string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret) && !string.IsNullOrWhiteSpace(tenant);
        }
    }
}