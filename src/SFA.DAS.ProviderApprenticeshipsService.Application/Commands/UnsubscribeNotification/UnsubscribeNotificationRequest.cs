using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UnsubscribeNotification;

public class UnsubscribeNotificationRequest : IRequest
{
    public string UserRef { get; set; }
}