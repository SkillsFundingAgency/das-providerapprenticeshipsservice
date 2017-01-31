using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class ProviderAgreementStatusRepository : DbBaseRepository, IProviderAgreementStatusRepository, IAgreementStatusQueryRepository
    {
        private readonly ILog _logger;

        public ProviderAgreementStatusRepository(IConfiguration config, ILog logger) : base(config.DatabaseConnectionString)
        {
            _logger = logger;
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
                          + "FROM [dbo].[ContractFeedEvent] "
                          + "ORDER BY [PageNumber] desc",
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
    }
}
