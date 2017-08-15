using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateUserNotificationSettings
{
    public class UpdateUserNotificationSettingsCommand : IAsyncRequest
    {
        public string UserRef { get; set; }

        public bool ReceiveNotifications { get; set; }
    }
}