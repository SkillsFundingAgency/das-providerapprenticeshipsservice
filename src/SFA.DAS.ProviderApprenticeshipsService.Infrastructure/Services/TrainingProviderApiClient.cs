using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

/// <inheritdoc />
public class TrainingProviderApiClient : ITrainingProviderApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TrainingProviderApiClientConfiguration _config;
    private readonly ILogger<TrainingProviderApiClient> _logger;

    public TrainingProviderApiClient(
        HttpClient httpClient,
        TrainingProviderApiClientConfiguration config,
        ILogger<TrainingProviderApiClient> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<GetProviderSummaryResult> GetProviderDetails(long providerId)
    {
        _logger.LogInformation("Getting Training Provider Details for ukprn:{0} returned OK", providerId);

        var url = $"{BaseUrl()}api/providers/{providerId}";
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

        await AddAuthenticationHeader(requestMessage);
        
        var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                _logger.LogInformation("{Url} returned OK", url);
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<GetProviderSummaryResult>(json);
            case HttpStatusCode.NotFound:
                _logger.LogInformation("{Url} returned not found status code", url);
                return default;
            default:
                _logger.LogError("{Url} returned unexpected status code", url);
                return default;
        }
    }

    #region "Private Methods"
    private string BaseUrl()
    {
        if (_config.ApiBaseUrl.EndsWith("/"))
        {
            return _config.ApiBaseUrl;
        }
        return _config.ApiBaseUrl + "/";
    }

    private async Task AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
    {
        if (!string.IsNullOrEmpty(_config.IdentifierUri))
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(_config.IdentifierUri);
            httpRequestMessage.Headers.Remove("X-Version");
            httpRequestMessage.Headers.Add("X-Version", "1.0");
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
    #endregion
}