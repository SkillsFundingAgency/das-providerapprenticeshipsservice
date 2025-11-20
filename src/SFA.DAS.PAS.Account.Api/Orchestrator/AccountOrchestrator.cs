using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;

namespace SFA.DAS.PAS.Account.Api.Orchestrator;
public interface IAccountOrchestrator
{
    Task<IEnumerable<User>> GetAccountUsers(long ukprn);
}

public class AccountOrchestrator : IAccountOrchestrator
{
    private readonly IMediator _mediator;

    public AccountOrchestrator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IEnumerable<User>> GetAccountUsers(long ukprn)
    {
        var result = await _mediator.Send(new GetAccountUsersQuery { Ukprn = ukprn });

        return result.UserSettings.Select(
            m =>
                new User
                {
                    EmailAddress = m.User.Email,
                    DisplayName = m.User.DisplayName,
                    ReceiveNotifications = m.Setting?.ReceiveNotifications ?? true,
                    UserRef = m.User.UserRef,
                    IsSuperUser = m.User.UserType == UserType.SuperUser
                });
    }
}