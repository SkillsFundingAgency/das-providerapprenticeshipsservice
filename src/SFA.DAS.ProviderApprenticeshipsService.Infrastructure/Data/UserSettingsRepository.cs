using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Dapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserSetting;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

public class UserSettingsRepository : BaseRepository<UserSettingsRepository>, IUserSettingsRepository
{
    public UserSettingsRepository(IBaseConfiguration configuration, ILogger<UserSettingsRepository> logger) 
        : base(configuration.DatabaseConnectionString, logger) { }

    public async Task<IEnumerable<UserSetting>> GetUserSetting(string userRef, string email)
    {
        return await WithConnection(async connection =>
        {
            IEnumerable<UserSetting> userSettings = null;
            var parameters = new DynamicParameters();
            var sql = "SELECT * FROM [dbo].[UserSettings] WHERE UserRef = @userRef";
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
            
            sql = "SELECT top 1 us.* FROM [dbo].[UserSettings] us inner join [dbo].[User] u on u.id = us.userid " +
                  "WHERE u.Email = @email order by lastlogin desc";
            parameters.Add("@email", email, DbType.String);  
            userSettings = await connection.QueryAsync<UserSetting>(
                sql: sql,
                param: parameters,
                commandType: CommandType.Text);

            return userSettings;
        });
    }
    
    public async Task AddSettings(string email)
    {
        await WithConnection(async connection =>
        {
            var parameters = new DynamicParameters();
            parameters.Add("@email", email, DbType.String);

            const string sql = @"INSERT INTO [dbo].[UserSettings] (UserId, UserRef)
                                SELECT TOP 1 
                                    [Id] As UserId
                                    ,[UserRef] 
                                FROM [dbo].[User] 
                                WHERE Email = @email 
                                ORDER BY lastlogin DESC";

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

            return await connection.ExecuteAsync(
                sql: "UPDATE [dbo].[UserSettings] "
                     + "SET ReceiveNotifications = @receiveNotifications "
                     + "WHERE UserRef = (Select top 1 UserRef from [dbo].[User]  where email = @email order by lastlogin desc)",
                param: parameters,
                commandType: CommandType.Text);
        });
    }
}