using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UnsubscribeNotification
{
    public class UnsubscribeNotificationRequest : IRequest<Unit>
    {
        public string UserRef { get; set; }
    }
}
