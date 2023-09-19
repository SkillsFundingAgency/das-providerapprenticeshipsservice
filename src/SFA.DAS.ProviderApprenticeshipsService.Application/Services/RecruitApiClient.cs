using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services
{
    public class RecruitApiClient : IRecruitApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly RecruitApiConfiguration _config;

        public RecruitApiClient(
            HttpClient httpClient,
            IOptions<RecruitApiConfiguration> config)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        }

        public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
            AddHeaders(requestMessage);

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            if (response.StatusCode.Equals(HttpStatusCode.NotFound))
            {
                return default;
            }

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<TResponse>(json);
            }

            response.EnsureSuccessStatusCode();

            return default;
        }

        private void AddHeaders(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _config.Key);
            httpRequestMessage.Headers.Add("X-Version", "1");
        }
    }
}
