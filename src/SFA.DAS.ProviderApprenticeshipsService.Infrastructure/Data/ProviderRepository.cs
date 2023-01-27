using Dapper;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using NLog;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class ProviderRepository : BaseRepository<ProviderRepository>, IProviderRepository
    {
        public ProviderRepository(ProviderApprenticeshipsServiceConfiguration config, ILogger<ProviderRepository> logger) : base(
            config.DatabaseConnectionString, logger)
        {
        }

        public async Task ImportProviders(DataTable providersDataTable)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@providers", providersDataTable.AsTableValuedParameter());
                parameters.Add("@now", DateTime.UtcNow, DbType.DateTime2);

                return await c.ExecuteAsync(
                    sql: "[dbo].[ImportProviders]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<Provider> GetNextProviderForIdamsUpdate()
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<Provider>(
                    "SELECT TOP 1 [Ukprn], [Name], [Created], [Updated], [UpdatedFromIDAMS] FROM [dbo].[Providers] P WHERE EXISTS(SELECT * FROM [dbo].[User] U WHERE U.[Ukprn] = P.[Ukprn]) ORDER BY [UpdatedFromIDAMS];",
                    commandType: CommandType.Text);
                return result.SingleOrDefault();
            });
        }

        public async Task MarkProviderIdamsUpdated(long ukprn)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ukprn", ukprn, DbType.Int64);

                return await c.ExecuteAsync(
                    sql: "UPDATE [dbo].[Providers] "
                         + "SET UpdatedFromIDAMS = GETDATE() "
                         + "WHERE Ukprn = @ukprn",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }
    }
}
