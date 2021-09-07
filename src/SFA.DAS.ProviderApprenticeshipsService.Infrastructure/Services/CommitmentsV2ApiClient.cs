using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class CommitmentsV2ApiClient : ApiClientBase, ICommitmentsV2ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly CommitmentsApiClientV2Configuration _config;
        private readonly ILogger<CommitmentsV2ApiClient> _logger;

        public CommitmentsV2ApiClient(HttpClient httpClient, CommitmentsApiClientV2Configuration config, ILogger<CommitmentsV2ApiClient> logger) : base(httpClient)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<GetCohortResponse> GetCohort(long cohortId)
        {
            var url = $"{BaseUrl()}api/cohorts/{cohortId}";
            var response = JsonConvert.DeserializeObject<GetCohortResponse>(await GetAsync(url));
            return response;
        }

        public async Task<bool> ApprenticeEmailRequired(long providerId)
        {
            var url = $"{BaseUrl()}api/authorization/features/providers/{providerId}/apprentice-email-required";
            _logger.LogInformation($"Getting {url}");
            var response = await _httpClient.GetAsync(url);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation($"{url} returned OK");
                    return true;
                case HttpStatusCode.NotFound:
                    _logger.LogInformation($"{url} returned NotFound");
                    return false;
                default:
                    _logger.LogError($"{url} returned unexpected status code");
                    throw new ApplicationException("Unexpected status code returned");
            }
        }       

        public async Task<bool> OptionalEmail(long providerId, long employerId)
        {
            var url = $"{BaseUrl()}api/authorization/email-optional?providerId={providerId}&employerid={employerId}";
            _logger.LogInformation($"Getting {url}");
            var response = await _httpClient.GetAsync(url);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation($"{url} returned OK");
                    return true;
                case HttpStatusCode.NotFound:
                    _logger.LogInformation($"{url} returned NotFound");
                    return false;
                default:
                    _logger.LogError($"{url} returned unexpected status code");
                    throw new ApplicationException("Unexpected status code returned");
            }
        }

        private string BaseUrl()
        {
            if (_config.ApiBaseUrl.EndsWith("/"))
            {
                return _config.ApiBaseUrl;
            }

            return _config.ApiBaseUrl + "/";
        }
    }
}
