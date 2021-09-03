using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings
{
    public class NotificationSettingsViewModel : ViewModelBase
    {
        public int ProviderId { get; set; }

        public IList<UserNotificationSetting> NotificationSettings { get; set; }
    }
}