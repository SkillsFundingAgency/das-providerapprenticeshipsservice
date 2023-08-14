using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;

namespace SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsResponse
{
    public List<UserNotificationSetting>? NotificationSettings { get; set; }
}