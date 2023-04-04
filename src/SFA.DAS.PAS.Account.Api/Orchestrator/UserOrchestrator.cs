using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.PAS.Account.Application.Queries.GetUser;
using SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.Account.Api.Orchestrator;

public interface IUserOrchestrator
{
    Task<User> GetUserWithSettings(string userRef);
}
public class UserOrchestrator : IUserOrchestrator
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserOrchestrator> _logger;

    public UserOrchestrator(IMediator mediator, ILogger<UserOrchestrator> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<User?> GetUserWithSettings(string userRef)
    {
        var user = await GetUserDetails(userRef);

        if (user.UserRef == null)
        {
            return null;
        };

        var userSetting = await GetUserSetting(userRef);

        if (userSetting == null)
        {
            return new User
            {
                UserRef = user.UserRef,
                EmailAddress = user.EmailAddress,
                DisplayName = user.Name,
                IsSuperUser = user.IsSuperUser
            };
        };

        return new User
        {
            UserRef = user.UserRef,
            EmailAddress = user.EmailAddress,
            DisplayName = user.Name,
            ReceiveNotifications = userSetting.ReceiveNotifications,
            IsSuperUser = user.IsSuperUser
        };
    }

    private async Task<UserNotificationSetting?> GetUserSetting(string userRef)
    {
        var userSetting = await _mediator.Send(new GetUserNotificationSettingsQuery { UserRef = userRef });

        var setting = userSetting.NotificationSettings.SingleOrDefault();
        if (setting == null)
        {
            _logger.LogInformation($"Unable to get user settings with ref {userRef}");
            return null;
        }

        return setting;
    }

    private async Task<GetUserQueryResponse> GetUserDetails(string userRef)
    {
        var user = await _mediator.Send(new GetUserQuery { UserRef = userRef });

        return user;
    }
}
