using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class CommitmentsV2ApiClient : ApiClientBase, ICommitmentsV2ApiClient
    {
        private readonly CommitmentsApiClientV2Configuration _config;

        public CommitmentsV2ApiClient(HttpClient httpClient, CommitmentsApiClientV2Configuration config) : base(httpClient)
        {
            _config = config;
        }

        public async Task<GetCohortResponse> GetCohort(long cohortId)
        {
            var url = $"{BaseUrl()}api/cohorts/{cohortId}";
            var response = JsonConvert.DeserializeObject<GetCohortResponse>(await GetAsync(url));
            return response;
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
