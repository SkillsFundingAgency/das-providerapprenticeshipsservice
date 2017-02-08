using MediatR;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification
{
    public class SendNotificationCommand : IAsyncRequest
    {
        public Email Email { get; set; }
    }
}