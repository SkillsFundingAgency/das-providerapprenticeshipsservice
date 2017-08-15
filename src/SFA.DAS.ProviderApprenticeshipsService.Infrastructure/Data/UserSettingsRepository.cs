using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Dapper;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class UserSettingsRepository : BaseRepository, IUserSettingsRepository
    {
        private readonly ILog _logger;

        public UserSettingsRepository(IConfiguration config, ILog logger) : base(config.DatabaseConnectionString, logger)
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
    }
}