using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.PAS.Account.Application.Queries.GetUser;
using SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;

namespace SFA.DAS.PAS.Account.Api.Orchestrator;

public interface IUserOrchestrator
{
    Task<User> GetUserWithSettings(string userRef);
}

public class UserOrchestrator(IMediator mediator, ILogger<UserOrchestrator> logger) : IUserOrchestrator
{
    public async Task<User> GetUserWithSettings(string userRef)
    {
        var userResponse = await mediator.Send(new GetUserQuery { UserRef = userRef });

        if (string.IsNullOrEmpty(userResponse.UserRef))
        {
            return null;
        }

        var user = new User
        {
            UserRef = userResponse.UserRef,
            EmailAddress = userResponse.EmailAddress,
            DisplayName = userResponse.Name
        };

        var userSetting = await GetUserSetting(userRef, userResponse.EmailAddress);

        if (userSetting != null)
        {
            user.ReceiveNotifications = userSetting.ReceiveNotifications;
        }

        return user;
    }

    private async Task<UserNotificationSetting> GetUserSetting(string userRef, string email)
    {
        var userSetting = await mediator.Send(new GetUserNotificationSettingsQuery { UserRef = userRef, Email = email});

        var setting = userSetting.NotificationSettings.SingleOrDefault();
        
        if (setting == null)
        {
            logger.LogInformation("Unable to get user settings with ref {UserRef}", userRef);
            return null;
        }

        return setting;
    }
}
