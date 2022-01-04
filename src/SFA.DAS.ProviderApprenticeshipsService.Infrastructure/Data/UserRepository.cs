using MoreLinq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly ILog _logger;       

        public UserRepository(ProviderApprenticeshipsServiceConfiguration config, ILog logger) : base(config.DatabaseConnectionString, logger)
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

        public async Task<User> GetUser(string userRef)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userRef", userRef, DbType.String);

                var results =
                    await
                    c.QueryAsync<User>(
                        sql: "SELECT TOP 1 * FROM [dbo].[User] WHERE UserRef = @userRef",
                        param: parameters,
                        commandType: CommandType.Text);

                return results.Single();
            });
        }

        public async Task<IEnumerable<User>> GetUsers(long ukprn)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ukprn", ukprn, DbType.Int64);

                var results =
                    await
                    c.QueryAsync<User>(
                        sql: "SELECT * FROM [dbo].[User] WHERE Ukprn = @ukprn AND IsDeleted=0",
                        param: parameters,
                        commandType: CommandType.Text);

                return results;
            });
        }

        public async Task DeleteUser(string userRef)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userRef", userRef, DbType.String);

                return await c.ExecuteAsync(
                    sql: "UPDATE [dbo].[User] set [IsDeleted]=1 WHERE UserRef = @userRef",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task SyncIdamsUsers(long ukprn, List<IdamsUser> idamsUsers)
        {
            var sublist = idamsUsers.Select(x => new {x.Email, UserType = (short) x.UserType}).ToDataTable(u => u.Email, u => u.UserType);

            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ukprn", ukprn, DbType.Int64);
                parameters.Add("@users", sublist.AsTableValuedParameter());

                return await c.ExecuteAsync(
                    sql: "[dbo].[SyncIdamsUsers]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

    }
}
