using Dapper;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Sql.Client;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class ImportProviderRepository : BaseRepository, IImportProviderRepository
    {
        private ILog _logger;

        public ImportProviderRepository(IImportProviderConfiguration importProviderConfiguarion , ILog logger): base(importProviderConfiguarion.DatabaseConnectionString, logger)
        {
            _logger = logger;
        }

        public async Task ImportProviders(DataTable providersDataTable)
        {
            await WithConnection(async c =>
            {
                var providers = new SqlParameter("providers", SqlDbType.Structured)
                {
                    TypeName = "Providers",
                    Value = providersDataTable
                };
                var now = new SqlParameter("now", DateTime.UtcNow);

                var parameters = new DynamicParameters();
                parameters.Add("@providers", providersDataTable.AsTableValuedParameter());
                parameters.Add("@now", DateTime.UtcNow, DbType.DateTime2);

                return await c.ExecuteAsync(
                  sql: "[dbo].[ImportProviders]",
                  param: parameters,
                  commandType: CommandType.StoredProcedure);
            });
        }
    }
}
