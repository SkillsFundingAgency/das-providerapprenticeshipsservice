using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
//using SFA.DAS.Sql.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class UserSettingsRepository : BaseRepository, IUserSettingsRepository
    {
        private readonly ILog _logger;

        public UserSettingsRepository(ProviderApprenticeshipsServiceConfiguration config, ILog logger) : base(config.DatabaseConnectionString, logger, ConfigurationManager.AppSettings["EnvironmentName"].Equals("LOCAL"))
        {
            _logger = logger;
        }

        public async Task<IEnumerable<UserSetting>> GetUserSetting(string userRef)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userRef", userRef, DbType.String);

                return await c.QueryAsync<UserSetting>(
                    sql: "SELECT * FROM [dbo].[UserSettings] WHERE UserRef = @userRef",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task AddSettings(string userRef)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userRef", userRef, DbType.String);

                return await c.ExecuteAsync(
                    sql: "INSERT INTO [dbo].[UserSettings] (UserId, UserRef) "
                         + "SELECT[Id] as UserId,[UserRef] FROM[dbo].[User] "
                         + "WHERE UserRef = @userRef",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task UpdateUserSettings(string userRef, bool receiveNotifications)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userRef", userRef, DbType.String);
                parameters.Add("@receiveNotifications", receiveNotifications, DbType.Boolean);

                return await c.ExecuteAsync(
                    sql: "UPDATE [dbo].[UserSettings] "
                         + "SET ReceiveNotifications = @receiveNotifications "
                         + "WHERE UserRef = @userRef",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }
    }
}