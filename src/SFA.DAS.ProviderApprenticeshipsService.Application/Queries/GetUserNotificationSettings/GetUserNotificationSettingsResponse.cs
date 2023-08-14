using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;
using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsResponse
{
    public List<UserNotificationSetting> NotificationSettings { get; set; }
}