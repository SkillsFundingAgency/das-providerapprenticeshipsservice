using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateUserNotificationSettings;

public class UpdateUserNotificationSettingsCommand : IRequest
{
    public string Email { get; set; }

    public bool ReceiveNotifications { get; set; }
}