using Newtonsoft.Json;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    /// <summary>
    /// Class responsible for DfESignIn Service.
    /// </summary>
    public class DfESignInService : ApiClientBase, IDfESignInService
    {
        private readonly HttpClient _httpClient;
        private readonly IDfESignInServiceConfiguration _config;

        public DfESignInService(HttpClient client, IDfESignInServiceConfiguration config) : base(client)
        {
            _httpClient = client;
            _config = config;
        }

        /// <inheritdoc  />
        public async Task<T> Get<T>(string userId, string userOrgId)
        {
            var endpoint = GetEndPoint(userId, userOrgId);
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var getResponse = await _httpClient.SendAsync(request);
            return getResponse.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(await getResponse.Content.ReadAsStringAsync()) : default;
        }

        /// <summary>
        /// Method to generate the endpoint from configuration.
        /// </summary>
        /// <param name="userOrgId">User Org Id.</param>
        /// <param name="userId">User Id.</param>
        /// <returns>string.</returns>
        private string GetEndPoint(string userId, string userOrgId)
        {
            return $"{_config.DfEOidcConfiguration.ApiServiceUrl}/services/{_config.DfEOidcConfiguration.ApiServiceId}/organisations/{userOrgId}/users/{userId}";
        }
    }
}
