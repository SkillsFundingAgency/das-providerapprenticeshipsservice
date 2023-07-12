using Dapper;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using Provider = SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Provider;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

public class ProviderRepository : BaseRepository<ProviderRepository>, IProviderRepository
{
    public ProviderRepository(IBaseConfiguration configuration, ILogger<ProviderRepository> logger, IConfiguration rootConfig, ChainedTokenCredential chainedTokenCredential) 
        : base(configuration.DatabaseConnectionString, logger) { }

    public async Task ImportProviders(CommitmentsV2.Api.Types.Responses.Provider[] providers)
    {
        var providersDataTable = new DataTable();
        providersDataTable.Columns.Add("Ukprn");
        providersDataTable.Columns.Add("Name");

        foreach (var provider in providers)
        {
            providersDataTable.Rows.Add(provider.Ukprn, provider.Name);
        }

        await WithConnection(async connection =>
        {
            var parameters = new DynamicParameters();
            parameters.Add("@providers", providersDataTable.AsTableValuedParameter());
            parameters.Add("@now", DateTime.UtcNow, DbType.DateTime2);

            return await connection.ExecuteAsync(
                sql: "[dbo].[ImportProviders]",
                param: parameters,
                commandType: CommandType.StoredProcedure);
        });
    }

    public async Task<Provider> GetNextProviderForIdamsUpdate()
    {
        return await WithConnection(async connection =>
        {
            var result = await connection.QueryAsync<Provider>(
                "SELECT TOP 1 [Ukprn], [Name], [Created], [Updated], [UpdatedFromIDAMS] FROM [dbo].[Providers] P WHERE EXISTS(SELECT * FROM [dbo].[User] U WHERE U.[Ukprn] = P.[Ukprn]) ORDER BY [UpdatedFromIDAMS];",
                commandType: CommandType.Text);
            return result.SingleOrDefault();
        });
    }

    public async Task MarkProviderIdamsUpdated(long ukprn)
    {
        await WithConnection(async connection =>
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ukprn", ukprn, DbType.Int64);

            return await connection.ExecuteAsync(
                sql: "UPDATE [dbo].[Providers] "
                     + "SET UpdatedFromIDAMS = GETDATE() "
                     + "WHERE Ukprn = @ukprn",
                param: parameters,
                commandType: CommandType.Text);
        });
    }
}