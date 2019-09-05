using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.PAS.Account.Api.ClientV2
{
    public class PasAccountApiClientFactory : IPasAccountApiClientFactory
    {
        private readonly IAzureActiveDirectoryClientConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public PasAccountApiClientFactory(IAzureActiveDirectoryClientConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IPasAccountApiClient CreateClient()
        {
            var httpClientFactory = new AzureActiveDirectoryHttpClientFactory(_configuration, _loggerFactory);
            var httpClient = httpClientFactory.CreateHttpClient();
            var restHttpClient = new RestHttpClient(httpClient);
            var apiClient = new PasAccountApiClient(restHttpClient);

            return apiClient;
        }
    }
}