namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings
{
    public class UserNotificationSetting
    {
        public string UserRef { get; set; }

        public string Email { get; set; }

        public bool ReceiveNotifications { get; set; }
    }
}