using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings
{
    public class NotificationSettingsViewModel : ViewModelBase
    {
        public string HashedId { get; set; }
        public string Email { get; set; }
        public IList<UserNotificationSetting> NotificationSettings { get; set; }
    }
}