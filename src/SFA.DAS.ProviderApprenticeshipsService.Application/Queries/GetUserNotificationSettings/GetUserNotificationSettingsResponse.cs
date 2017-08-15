using System.Collections.Generic;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings
{
    public class GetUserNotificationSettingsResponse
    {
        public List<UserNotificationSetting> NotificationSettings { get; set; }
    }
}