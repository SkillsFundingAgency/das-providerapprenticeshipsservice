using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IUserSettingsRepository
    {
        Task<IEnumerable<UserSetting>> GetUserSetting(string userRef);

        Task AddSettings(string userRef);

        Task UpdateUserSettings(string userRef, bool receiveNotifications);
    }
}