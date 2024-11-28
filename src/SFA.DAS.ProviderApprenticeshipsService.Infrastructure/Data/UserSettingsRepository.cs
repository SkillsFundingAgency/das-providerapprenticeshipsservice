using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserSetting;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

public class UserSettingsRepository(
    IBaseConfiguration configuration,
    ILogger<UserSettingsRepository> logger)
    : BaseRepository<UserSettingsRepository>(configuration.DatabaseConnectionString, logger), IUserSettingsRepository
{
    public async Task<IEnumerable<UserSetting>> GetUserSetting(string userRef, string email)
    {
        return await WithConnection(async connection =>
        {
            IEnumerable<UserSetting> userSettings = null;
            var parameters = new DynamicParameters();

            var sql = @"SELECT
	                        UserId
	                        ,UserRef
	                        ,ReceiveNotifications
                        FROM [dbo].[UserSettings]  
                        WHERE UserRef = @userRef";

            if (!string.IsNullOrEmpty(userRef))
            {
                parameters.Add("@userRef", userRef, DbType.String);
                userSettings = await connection.QueryAsync<UserSetting>(
                    sql: sql,
                    param: parameters,
                    commandType: CommandType.Text);
            }

            if (userSettings != null && userSettings.Any())
            {
                return userSettings;
            }

            sql = @"SELECT TOP 1 
	                    us.UserId
	                    ,us.UserRef
	                    ,ReceiveNotifications
                    FROM [dbo].[UserSettings] us 
                    INNER JOIN [dbo].[User] u ON u.Id = us.Userid 
                    WHERE u.Email = @email 
                    ORDER BY LastLogin DESC";

            parameters.Add("@email", email, DbType.String);

            userSettings = await connection.QueryAsync<UserSetting>(
                sql: sql,
                param: parameters,
                commandType: CommandType.Text);

            return userSettings;
        });
    }

    public async Task AddSettings(string email, bool receiveNotifications = false)
    {
        await WithConnection(async connection =>
        {
            var parameters = new DynamicParameters();
            parameters.Add("@email", email, DbType.String);
            parameters.Add("@receiveNotifications", receiveNotifications, DbType.Boolean);

            const string sql = @"INSERT INTO [dbo].[UserSettings] (UserId, UserRef, ReceiveNotifications)
                                SELECT TOP 1
	                                [Id] As UserId
	                                ,[UserRef]
	                                ,@receiveNotifications AS ReceiveNotifications
                                FROM [dbo].[User]
                                WHERE Email = @Email
                                ORDER BY LastLogin DESC";

            return await connection.ExecuteAsync(
                sql: sql,
                param: parameters,
                commandType: CommandType.Text);
        });
    }

    public async Task UpdateUserSettings(string email, bool receiveNotifications)
    {
        await WithConnection(async connection =>
        {
            var parameters = new DynamicParameters();
            parameters.Add("@email", email, DbType.String);
            parameters.Add("@receiveNotifications", receiveNotifications, DbType.Boolean);

            const string sql = @"UPDATE [dbo].[UserSettings] 
                                SET 
                                    ReceiveNotifications = @receiveNotifications 
                                WHERE UserRef = 
                                (
                                    SELECT TOP 1 
                                        UserRef 
                                    FROM [dbo].[User] 
                                    WHERE Email = @email
                                    ORDER BY LastLogin DESC
                                )";

            return await connection.ExecuteAsync(
                sql: sql,
                param: parameters,
                commandType: CommandType.Text);
        });
    }
}