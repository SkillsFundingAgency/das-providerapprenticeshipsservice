using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserSetting;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

public interface IUserSettingsRepository
{
    Task<IEnumerable<UserSetting>> GetUserSetting(string userRef, string email);

    Task AddSettings(string email, bool receiveNotifications = false);

    Task UpdateUserSettings(string email, bool receiveNotifications);
}