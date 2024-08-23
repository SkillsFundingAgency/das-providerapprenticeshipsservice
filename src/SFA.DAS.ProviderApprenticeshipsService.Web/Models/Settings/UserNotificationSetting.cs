namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings;

public class UserNotificationSetting
{
    public string Email { get; set; }
    public string UserRef { get; set; }

    public bool ReceiveNotifications { get; set; }
    public string Name { get; set; }
}