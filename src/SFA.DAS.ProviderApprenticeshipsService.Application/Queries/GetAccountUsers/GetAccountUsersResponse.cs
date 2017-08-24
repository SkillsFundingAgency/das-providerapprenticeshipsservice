using System.Collections.Generic;

using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAccountUsers
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