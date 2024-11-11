using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.PAS.Account.Application.Queries.GetProviderAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using ProviderAgreementStatus = SFA.DAS.PAS.Account.Api.Types.ProviderAgreementStatus;

namespace SFA.DAS.PAS.Account.Api.Orchestrator;

public interface IAccountOrchestrator
{
    Task<IEnumerable<User>> GetAccountUsers(long ukprn);
    Task<ProviderAgreement> GetAgreement(long providerId);
}

public class AccountOrchestrator(IMediator mediator) : IAccountOrchestrator
{
    public async Task<IEnumerable<User>> GetAccountUsers(long ukprn)
    {
        var result = await mediator.Send(new GetAccountUsersQuery { Ukprn = ukprn });

        return result.UserSettings.Select(m => new User
            {
                EmailAddress = m.User.Email,
                DisplayName = m.User.DisplayName,
                ReceiveNotifications = m.Setting?.ReceiveNotifications ?? true,
                UserRef = m.User.UserRef
            }
        );
    }

    public async Task<ProviderAgreement> GetAgreement(long providerId)
    {
        var data = await mediator.Send(
            new GetProviderAgreementQueryRequest
            {
                ProviderId = providerId
            });

        return new ProviderAgreement
        {
            Status = (ProviderAgreementStatus)Enum.Parse(typeof(ProviderAgreementStatus), data.HasAgreement.ToString())
        };
    }
}