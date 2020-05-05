using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Http;
using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.PAS.Account.Api.ClientV2
{
    public class PasAccountApiClient : IPasAccountApiClient
    {
        private readonly IRestHttpClient _httpClient;

        public PasAccountApiClient(IRestHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<User> GetUser(string userRef, CancellationToken cancellationToken)
        {
            return _httpClient.Get<User>($"api/user/{userRef}", null, cancellationToken);
        }

        public Task<IEnumerable<User>> GetAccountUsers(long providerId, CancellationToken cancellationToken)
        {
            return _httpClient.Get<IEnumerable<User>>($"api/account/{providerId}/users", null, cancellationToken);
        }

        public Task SendEmailToAllProviderRecipients(long providerId, ProviderEmailRequest message, CancellationToken cancellationToken)
        {
            return _httpClient.PostAsJson($"api/email/{providerId}/send", message, cancellationToken);
        }

        public Task<ProviderAgreement> GetAgreement(long providerId, CancellationToken cancellationToken = default)
        {
            return _httpClient.Get<ProviderAgreement>($"api/account/{providerId}/agreement", null, cancellationToken);
        }
    }
}
