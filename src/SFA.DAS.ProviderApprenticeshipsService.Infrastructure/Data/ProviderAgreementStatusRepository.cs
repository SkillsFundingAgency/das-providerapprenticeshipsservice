using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class ProviderAgreementStatusRepository : BaseRepository, IProviderAgreementStatusRepository, IAgreementStatusQueryRepository
    {
        private readonly ILog _logger;
        private readonly ICurrentDateTime _currentDateTime;

        public ProviderAgreementStatusRepository(IProviderAgreementStatusConfiguration config, ILog logger, ICurrentDateTime currentDateTime) : base(config.DatabaseConnectionString, logger)
        {
            _logger = logger;
            _currentDateTime = currentDateTime;
        }

        public async Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId)
        {
            var contractFeedEvents = await WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@providerId", providerId, DbType.Int64);

                using (var trans = connection.BeginTransaction())
                {
                    var r = (await connection.QueryAsync<ContractFeedEvent>(
                        sql:
                            "SELECT * FROM [dbo].[ContractFeedEvent] "
                          + "WHERE [ProviderId] = @providerId",
                        param: parameters,
                        commandType: CommandType.Text,
                        transaction: trans));
                    trans.Commit();
                    return r;
                }
            });

            return contractFeedEvents;
        }

        public async Task<Guid?> GetLatestBookmark()
        {
            var latestBookmark = await WithConnection(async connection =>
            {
                Guid? bookmark = await GetLatestBookmark(connection, null);

                return bookmark;
            });

            if (latestBookmark == null)
                _logger.Info("Latest Bookmark not found.");

            return latestBookmark;
        }

        private static async Task<Guid?> GetLatestBookmark(IDbConnection connection, IDbTransaction tran)
        {
            return (await connection.QueryAsync<Guid?>(
                                    sql:
                                        "SELECT [LatestBookmark] "
                                      + "FROM [dbo].[ContractFeedEventRun]",
                                    commandType: CommandType.Text, transaction: tran)).SingleOrDefault();
        }

        public async Task AddContractEventsForPage(IList<ContractFeedEvent> contractFeedEvents, Guid newBookmark)
        {
            await WithTransaction(async (conn, tran) =>
            {
                foreach (var contract in contractFeedEvents)
                {
                    await InsertContract(conn, tran, contract);
                }

                await UpdateLatestBookmark(conn, tran, newBookmark);
            });
        }

        private async Task UpdateLatestBookmark(IDbConnection conn, IDbTransaction tran, Guid newLatestBookmark)
        {
            _logger.Info($"Updating latest bookmark to: {newLatestBookmark.ToString()}");

            var parameters = new DynamicParameters();
            parameters.Add("@latestBookmark", newLatestBookmark, DbType.Guid);
            parameters.Add("@updatedDate", _currentDateTime.Now, DbType.DateTime);

            _logger.Info("Deleting existing record from [ContractFeedEventRun]");

            await conn.ExecuteAsync(
                sql:
                "DELETE FROM [dbo].[ContractFeedEventRun]",
                commandType: CommandType.Text,
                transaction: tran);

            _logger.Info("Inserting new record into [ContractFeedEventRun]");

            await conn.ExecuteAsync(
                sql:
                "INSERT INTO [dbo].[ContractFeedEventRun]" +
                "(LatestBookmark, UpdatedDate)" +
                "VALUES(@latestBookmark, @updatedDate); ",
                param: parameters,
                commandType: CommandType.Text,
                transaction: tran);
        }

        private async Task InsertContract(IDbConnection conn, IDbTransaction tran, ContractFeedEvent contract)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", contract.Id, DbType.Guid);
            parameters.Add("@providerId", contract.ProviderId, DbType.Int64);
            parameters.Add("@hierarchyType", contract.HierarchyType, DbType.String);
            parameters.Add("@fundingTypeCode", contract.FundingTypeCode, DbType.String);
            parameters.Add("@status", contract.Status, DbType.String);
            parameters.Add("@parentStatus", contract.ParentStatus, DbType.String);
            parameters.Add("@updatedInFeed", contract.Updated, DbType.DateTime);
            parameters.Add("@createdDate", _currentDateTime.Now, DbType.DateTime);

            await conn.ExecuteAsync(
                    sql:
                        "INSERT INTO [dbo].[ContractFeedEvent]" +
                        "(Id, ProviderId, HierarchyType, FundingTypeCode, Status, ParentStatus, UpdatedInFeed, CreatedDate)" +
                        "VALUES(@id, @providerId, @hierarchyType, @fundingTypeCode, @status, @parentStatus, @updatedInFeed, @createdDate); ",
                    param: parameters,
                    commandType: CommandType.Text,
                    transaction: tran);
        }

        public async Task<int> GetCountOfContracts()
        {
            return await WithConnection<int>(conn => 
            {
                return conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [dbo].[ContractFeedEvent];", commandType: CommandType.Text);
            });
        }
    }
}
