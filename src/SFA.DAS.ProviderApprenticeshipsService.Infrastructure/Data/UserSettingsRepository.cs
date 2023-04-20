using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserSetting;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

public class UserSettingsRepository : BaseRepository<UserSettingsRepository>, IUserSettingsRepository
{
    public UserSettingsRepository(IBaseConfiguration configuration, ILogger<UserSettingsRepository> logger, IConfiguration rootConfig) 
        : base(configuration.DatabaseConnectionString, logger, rootConfig) { }

    public async Task<IEnumerable<UserSetting>> GetUserSetting(string userRef)
    {
        return await WithConnection(async connection =>
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userRef", userRef, DbType.String);

            return await connection.QueryAsync<UserSetting>(
                sql: "SELECT * FROM [dbo].[UserSettings] WHERE UserRef = @userRef",
                param: parameters,
                commandType: CommandType.Text);
        });
    }

    public async Task AddSettings(string userRef)
    {
        await WithConnection(async connection =>
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userRef", userRef, DbType.String);

            return await connection.ExecuteAsync(
                sql: "INSERT INTO [dbo].[UserSettings] (UserId, UserRef) "
                     + "SELECT[Id] as UserId,[UserRef] FROM[dbo].[User] "
                     + "WHERE UserRef = @userRef",
                param: parameters,
                commandType: CommandType.Text);
        });
    }

    public async Task UpdateUserSettings(string userRef, bool receiveNotifications)
    {
        await WithConnection(async connection =>
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userRef", userRef, DbType.String);
            parameters.Add("@receiveNotifications", receiveNotifications, DbType.Boolean);

            return await connection.ExecuteAsync(
                sql: "UPDATE [dbo].[UserSettings] "
                     + "SET ReceiveNotifications = @receiveNotifications "
                     + "WHERE UserRef = @userRef",
                param: parameters,
                commandType: CommandType.Text);
        });
    }
}