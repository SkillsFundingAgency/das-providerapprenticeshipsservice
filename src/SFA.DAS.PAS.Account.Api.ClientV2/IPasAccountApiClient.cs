using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.PAS.Account.Api.ClientV2
{
    public interface IPasAccountApiClient
    {
        Task<User> GetUser(string userRef, CancellationToken cancellationToken = default);

        Task<IEnumerable<User>> GetAccountUsers(long ukprn, CancellationToken cancellationToken = default);

        Task SendEmailToAllProviderRecipients(long ukprn, ProviderEmailRequest message, CancellationToken cancellationToken = default);

        Task<ProviderAgreement> GetAgreement(long ukprn, CancellationToken cancellationToken = default);
    }
}