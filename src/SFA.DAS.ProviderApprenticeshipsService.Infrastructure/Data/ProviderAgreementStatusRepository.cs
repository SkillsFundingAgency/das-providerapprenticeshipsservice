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
    public class ProviderAgreementStatusRepository : DbBaseRepository, IProviderAgreementStatusRepository, IAgreementStatusQueryRepository
    {
        private readonly ILog _logger;
        private readonly ICurrentDateTime _currentDateTime;

        public ProviderAgreementStatusRepository(IConfiguration config, ILog logger, ICurrentDateTime currentDateTime) : base(config.DatabaseConnectionString)
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

        //public async Task<ContractFeedEvent> GetMostRecentContractFeedEvent()
        //{
        //    var contact = await WithConnection(async connection =>
        //    {
        //        using (var trans = connection.BeginTransaction())
        //        {
        //            var r = (await connection.QueryAsync<ContractFeedEvent>(
        //                sql:
        //                    "SELECT TOP 1 [Id], [PageNumber] "
        //                  + "FROM [dbo].[ContractFeedEvent] "
        //                  + "ORDER BY [Updated] desc",
        //                commandType: CommandType.Text,
        //                transaction: trans)).SingleOrDefault();
        //            trans.Commit();
        //            return r;
        //        }
        //    });

        //    if (contact == null)
        //        _logger.Info("No provider agreements found.");
        //    return contact;
        //}

        public async Task<Guid?> GetLatestBookmark()
        {
            var latestBookmark = await WithConnection(async connection =>
            {
                // TODO: LWA Is this the best way to do this??
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

        //public async Task AddContractEvent(ContractFeedEvent contractFeedEvent)
        //{
        //    await WithConnection(async connection =>
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("@id", contractFeedEvent.Id, DbType.Guid);
        //        parameters.Add("@providerId", contractFeedEvent.ProviderId, DbType.Int64);
        //        parameters.Add("@hierarchyType", contractFeedEvent.HierarchyType, DbType.String);
        //        parameters.Add("@fundingTypeCode", contractFeedEvent.FundingTypeCode, DbType.String);
        //        parameters.Add("@status", contractFeedEvent.Status, DbType.String);
        //        parameters.Add("@parentStatus", contractFeedEvent.ParentStatus, DbType.String);
        //        parameters.Add("@updatedInFeed", contractFeedEvent.Updated, DbType.DateTime);
        //        parameters.Add("@pageNumber", contractFeedEvent.PageNumber, DbType.Int32);
        //        parameters.Add("@createdDate", DateTime.UtcNow, DbType.DateTime);

        //        using (var trans = connection.BeginTransaction())
        //        {
        //            await connection.ExecuteAsync(
        //                sql:
        //                    "INSERT INTO [dbo].[ContractFeedEvent]" +
        //                    "(Id, ProviderId, HierarchyType, FundingTypeCode, Status, ParentStatus, UpdatedInFeed, PageNumber, CreatedDate)" +
        //                    "VALUES(@id, @providerId, @hierarchyType, @fundingTypeCode, @status, @parentStatus, @updatedInFeed, @pageNumber, @createdDate); ",
        //                param: parameters,
        //                commandType: CommandType.Text,
        //                transaction: trans);
        //            trans.Commit();
        //        }
        //        return 1L;
        //    }
        //    );
        //}

        public async Task AddContractEventsForPage(List<ContractFeedEvent> contractFeedEvents, Guid newBookmark)
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

        private static async Task UpdateLatestBookmark(IDbConnection conn, IDbTransaction tran, Guid newLatestBookmark)
        {
            var previousBookmark = await GetLatestBookmark(conn, tran);

            var parameters = new DynamicParameters();
            parameters.Add("@latestBookmark", newLatestBookmark, DbType.Guid);
            parameters.Add("@updatedDate", DateTime.UtcNow, DbType.DateTime);

            if (previousBookmark == null)
            {
                await conn.ExecuteAsync(
                    sql:
                        "INSERT INTO [dbo].[ContractFeedEventRun]" +
                        "(LatestBookmark, UpdatedDate)" +
                        "VALUES(@latestBookmark, @updatedDate); ",
                    param: parameters,
                    commandType: CommandType.Text,
                    transaction: tran);
            }
            else
            {
                await conn.ExecuteAsync(
                    sql:
                        "UPDATE [dbo].[ContractFeedEventRun]" +
                        "SET LatestBookmark = @latestBookmark, UpdatedDate = @updatedDate; ",
                    param: parameters,
                    commandType: CommandType.Text,
                    transaction: tran);
            }
        }

        private static async Task InsertContract(IDbConnection conn, IDbTransaction tran, ContractFeedEvent contract)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id", contract.Id, DbType.Guid);
            parameters.Add("@providerId", contract.ProviderId, DbType.Int64);
            parameters.Add("@hierarchyType", contract.HierarchyType, DbType.String);
            parameters.Add("@fundingTypeCode", contract.FundingTypeCode, DbType.String);
            parameters.Add("@status", contract.Status, DbType.String);
            parameters.Add("@parentStatus", contract.ParentStatus, DbType.String);
            parameters.Add("@updatedInFeed", contract.Updated, DbType.DateTime);
            parameters.Add("@createdDate", DateTime.UtcNow, DbType.DateTime);

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

        //public async Task SaveLastRun(EventRun lastRun)
        //{
        //    await WithConnection(async connection =>
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("@entriesSaved", lastRun.ContractCount, DbType.Int32);
        //        parameters.Add("@executionTimeMs", lastRun.ExecutionTimeMs, DbType.Int64);
        //        parameters.Add("@pageNumber", lastRun.NewLastReadPageNumber, DbType.Int32);
        //        parameters.Add("@pagesRead", lastRun.PagesRead, DbType.Int32);
        //        parameters.Add("@updated", DateTime.Now, DbType.DateTime);

        //        using (var trans = connection.BeginTransaction())
        //        {
        //            await connection.ExecuteAsync(
        //                sql:
        //                    "INSERT INTO [dbo].[ContractFeedEventRun]" +
        //                    "(EntriesSaved, ExecutionTimeMs, PageNumber, PagesRead, Updated)" +
        //                    "VALUES(@entriesSaved, @executionTimeMs, @pageNumber, @pagesRead, @updated); ",
        //                param: parameters,
        //                commandType: CommandType.Text,
        //                transaction: trans);
        //            trans.Commit();
        //        }
        //        return 1L;
        //    });
        //}
    }
}
