using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UnsubscribeNotification
{
    public class UnsubscribeNotificationRequest : IAsyncRequest
    {
        public string UserRef { get; set; }
    }
}
