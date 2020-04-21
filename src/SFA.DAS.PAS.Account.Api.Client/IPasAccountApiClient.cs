using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.PAS.Account.Api.Client
{
    public interface IPasAccountApiClient
    {
        Task<User> GetUser(string userRef);

        Task<IEnumerable<User>> GetAccountUsers(long ukprn);

        Task SendEmailToAllProviderRecipients(long ukprn, ProviderEmailRequest message);

        Task<ProviderAgreement> GetAgreement(long ukprn);
    }
}