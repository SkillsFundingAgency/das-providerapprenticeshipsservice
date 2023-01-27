using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;

namespace SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers
{
    public class GetAccountUsersResponse
    {
        public GetAccountUsersResponse()
        {
            UserSettings = new List<UserSettingPair>();
        }

        public ICollection<UserSettingPair> UserSettings { get; }

        public void Add(User user, UserSetting settings)
        {
            UserSettings.Add(
                new UserSettingPair
                {
                    User = user,
                    Setting = settings
                });
        }
    }

    public class UserSettingPair
    {
        public User User { get; set; }

        public UserSetting Setting { get; set; }
    }
}