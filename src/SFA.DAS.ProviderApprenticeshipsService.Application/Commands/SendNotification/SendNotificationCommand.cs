using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;

public class SendNotificationCommand : IRequest
{
    public NotificationEmail Email { get; set; }
}