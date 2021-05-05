using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class ProviderCommitmentsApiV2Client : ApiClientBase, IProviderCommitmentsApiV2Client
    {
        private readonly HttpClient _httpClient;

        public ProviderCommitmentsApiV2Client(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GetCohortResponse> GetCohort(long cohortId, CancellationToken cancellationToken = default)
        {
            var url = $"{_httpClient.BaseAddress}/api/cohorts/{cohortId}";
            var response = JsonConvert.DeserializeObject<GetCohortResponse>(await this.GetAsync(url));
            return response;
        }
    }
}
