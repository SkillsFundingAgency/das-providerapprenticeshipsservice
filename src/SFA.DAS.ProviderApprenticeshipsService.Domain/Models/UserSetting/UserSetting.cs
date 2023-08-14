namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserSetting;

public class UserSetting
{
    public long UserId { get; set; }
    public string UserRef { get; set; }
    public bool ReceiveNotifications { get; set; }
}