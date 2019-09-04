using Microsoft.Extensions.Logging;
using SFA.DAS.Http;

namespace SFA.DAS.PAS.Account.Api.Client
{
    public class PasAccountApiClientFactory
    {
        private readonly IPasAccountApiConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public PasAccountApiClientFactory(IPasAccountApiConfiguration configuration, ILoggerFactory loggerFactory)
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