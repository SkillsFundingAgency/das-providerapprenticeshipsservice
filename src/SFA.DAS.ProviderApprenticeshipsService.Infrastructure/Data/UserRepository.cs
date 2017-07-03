using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly ILog _logger;

        public UserRepository(IConfiguration config, ILog logger) : base(config.DatabaseConnectionString, logger)
        {
            _logger = logger;
        }

        public async Task Upsert(User user)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userRef", user.UserRef, DbType.String);
                parameters.Add("@displayName", user.DisplayName, DbType.String);
                parameters.Add("@ukprn", user.Ukprn, DbType.Int64);
                parameters.Add("@email", user.Email, DbType.String);
                return await c.ExecuteAsync(
                    sql: "[dbo].[UpsertUser]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }
    }
}
