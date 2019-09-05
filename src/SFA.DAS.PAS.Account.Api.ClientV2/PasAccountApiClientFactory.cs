using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;

namespace SFA.DAS.PAS.Account.Api.ClientV2
{
    internal class PasAccountApiClientFactory
    {
        const string StubBase = "https://sfa-stub-employeraccountapi.herokuapp.com/";

        private readonly PasAccountApiConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public PasAccountApiClientFactory(PasAccountApiConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IPasAccountApiClient CreateClient()
        {
            HttpClient httpClient;

            if (_configuration.UseStub)
            {
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(StubBase);
            }
            else
            {
                var httpClientFactory = new AzureActiveDirectoryHttpClientFactory(_configuration, _loggerFactory);
                httpClient = httpClientFactory.CreateHttpClient();
            }

            var restHttpClient = new RestHttpClient(httpClient);
            var apiClient = new PasAccountApiClient(restHttpClient);

            return apiClient;
        }
    }
}