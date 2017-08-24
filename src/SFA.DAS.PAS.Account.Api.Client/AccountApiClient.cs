using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.PAS.Account.Api.Client
{
    public class AccountApiClient : IAccountApiClient
    {
        private readonly IAccountApiConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public AccountApiClient(IAccountApiConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration);
        }

        public async Task<User> GetUser(string userRef)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/user/";

            var json = await _httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<User>(json);
        }

        public async Task<IEnumerable<User>> GetAccountUsers(long ukprn)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{ukprn}/users";

            var json = await _httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<IEnumerable<User>>(json);
        }

        private string GetBaseUrl()
        {
            return _configuration.ApiBaseUrl.EndsWith("/")
                ? _configuration.ApiBaseUrl
                : _configuration.ApiBaseUrl + "/";
        }
    }
}
