using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;

namespace SFA.DAS.PAS.Account.Api.Orchestrator;

public interface IUserOrchestrator
{
    Task<User> GetUser(string userRef);
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

    public async Task<User> GetUser(string userRef)
    {
        var userSetting = await _mediator.Send(new GetUserNotificationSettingsQuery {UserRef = userRef });

        var setting = userSetting.NotificationSettings.SingleOrDefault();
        if (setting == null)
        {
            _logger.LogInformation($"Unable to get user with ref {userRef}");
            return null;
        }

        return new User { UserRef = setting.UserRef, ReceiveNotifications = setting.ReceiveNotifications };
    }
}