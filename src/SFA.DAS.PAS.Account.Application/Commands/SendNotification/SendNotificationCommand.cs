using MediatR;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.Account.Application.Commands.SendNotification;

public class SendNotificationCommand : IRequest
{
    public NotificationEmail Email { get; }

    public SendNotificationCommand(NotificationEmail email)
    {
        Email = email;
    }
}