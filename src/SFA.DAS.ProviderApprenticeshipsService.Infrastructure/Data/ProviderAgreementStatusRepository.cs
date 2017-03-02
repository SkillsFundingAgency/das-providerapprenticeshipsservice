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

        public async Task<ContractFeedEvent> GetMostRecentContractFeedEvent()
        {
            var contact = await WithConnection(async connection =>
            {
                using (var trans = connection.BeginTransaction())
                {
                    var r = (await connection.QueryAsync<ContractFeedEvent>(
                        sql:
                            "SELECT TOP 1 [Id], [PageNumber] "
                          + "FROM [dbo].[ContractFeedEvent] "
                          + "ORDER BY [Updated] desc",
                        commandType: CommandType.Text,
                        transaction: trans)).SingleOrDefault();
                    trans.Commit();
                    return r;
                }
            });

            if (contact == null)
                _logger.Info("No provider agreements found.");
            return contact;
        }

        public async Task<int> GetMostRecentPageNumber()
        {
            var pageNumber = await WithConnection<int>(async connection =>
            {
                using (var trans = connection.BeginTransaction())
                {
                    var r = (await connection.QueryAsync<int>(
                        sql:
                            "SELECT TOP 1 [PageNumber] "
                          + "FROM [dbo].[ContractFeedEventRun] "
                          + "ORDER BY [Updated] desc",
                        commandType: CommandType.Text,
                        transaction: trans)).SingleOrDefault();
                    trans.Commit();
                    return r;
                }
            });

            if (pageNumber.Equals(0))
                _logger.Info("No provider agreements found.");
            return pageNumber;
        }

        public async Task AddContractEvent(ContractFeedEvent contractFeedEvent)
        {
            await WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", contractFeedEvent.Id, DbType.Guid);
                parameters.Add("@providerId", contractFeedEvent.ProviderId, DbType.Int64);
                parameters.Add("@hierarchyType", contractFeedEvent.HierarchyType, DbType.String);
                parameters.Add("@fundingTypeCode", contractFeedEvent.FundingTypeCode, DbType.String);
                parameters.Add("@status", contractFeedEvent.Status, DbType.String);
                parameters.Add("@parentStatus", contractFeedEvent.ParentStatus, DbType.String);
                parameters.Add("@updated", contractFeedEvent.Updated, DbType.DateTime);
                parameters.Add("@pageNumber", contractFeedEvent.PageNumber, DbType.Int32);

                using (var trans = connection.BeginTransaction())
                {
                    await connection.ExecuteAsync(
                        sql:
                            "INSERT INTO [dbo].[ContractFeedEvent]" +
                            "(Id, ProviderId, HierarchyType, FundingTypeCode, Status, ParentStatus, Updated, PageNumber)" +
                            "VALUES(@id, @providerId, @hierarchyType, @fundingTypeCode, @status, @parentStatus, @updated, @pageNumber); ",
                        param: parameters,
                        commandType: CommandType.Text,
                        transaction: trans);
                    trans.Commit();
                }
                return 1L;
            }
            );
        }

        public async Task SaveLastRun(EventRun lastRun)
        {
            await WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@entriesSaved", lastRun.ContractCount, DbType.Int32);
                parameters.Add("@executionTimeMs", lastRun.ExecutionTimeMs, DbType.Int64);
                parameters.Add("@pageNumber", lastRun.NewLastReadPageNumber, DbType.Int32);
                parameters.Add("@pagesRead", lastRun.PagesRead, DbType.Int32);
                parameters.Add("@updated", DateTime.Now, DbType.DateTime);

                using (var trans = connection.BeginTransaction())
                {
                    await connection.ExecuteAsync(
                        sql:
                            "INSERT INTO [dbo].[ContractFeedEventRun]" +
                            "(EntriesSaved, ExecutionTimeMs, PageNumber, PagesRead, Updated)" +
                            "VALUES(@entriesSaved, @executionTimeMs, @pageNumber, @pagesRead, @updated); ",
                        param: parameters,
                        commandType: CommandType.Text,
                        transaction: trans);
                    trans.Commit();
                }
                return 1L;
            });
        }
    }
}
