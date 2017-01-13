using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class ProviderAgreementStatusRepository : DbBaseRepository, IProviderAgreementStatusRepository
    {
        private readonly ILog _logger;

        private readonly List<ContractFeedEvent> _data;

        public ProviderAgreementStatusRepository(string connectionString, ILog logger) : base (connectionString)
        {
            _logger = logger;
            _data = new List<ContractFeedEvent>();
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

                        using (var trans = connection.BeginTransaction())
                        {
                            await connection.ExecuteAsync(
                                sql:
                                    "INSERT INTO [dbo].[ContractFeedEvent]" +
                                    "(Id, ProviderId, HierarchyType, FundingTypeCode, Status, ParentStatus, Updated)" +
                                    "VALUES(@id, @providerId, @hierarchyType, @fundingTypeCode, @status, @parentStatus, @updated); ",
                                param: parameters,
                                commandType: CommandType.Text,
                                transaction: trans);
                            trans.Commit();
                        }
                        return 1L;
                    }
            );
        }

        public async Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId)
        {
            var contractFeedEvents = await WithConnection<IEnumerable<ContractFeedEvent>>(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@providerId", providerId, DbType.Int64);

                using (var trans = connection.BeginTransaction())
                {
                    var r = (await connection.QueryAsync<ContractFeedEvent>(
                        sql:
                            "SELECT * FROM"
                          + "FROM [SFA.DAS.ProviderAgreementStatus.Database].[dbo].[ContractFeedEvent] "
                          + "WHERE [Id] = @id",
                        param: parameters,
                        commandType: CommandType.Text,
                        transaction: trans));
                    trans.Commit();
                    return r;
                }
            });

            return contractFeedEvents;
            throw new NotImplementedException($"Not implemented exception: {this.GetType()}");
            //return _data.Where(e => e.ProviderId == providerId);
        }

        public async Task<Guid> GetMostRecentBookmarkId()
        {
            // Do we want to cache value?
            var guid = await WithConnection<Guid>(async connection =>
                    {
                        using (var trans = connection.BeginTransaction())
                        {
                            var r = (await connection.QueryAsync<Guid>(
                                sql:
                                    "SELECT TOP 1 [Id] "
                                  + "FROM [SFA.DAS.ProviderAgreementStatus.Database].[dbo].[ContractFeedEvent] "
                                  + "ORDER BY [Updated] desc",
                                commandType: CommandType.Text,
                                transaction: trans)).SingleOrDefault();
                            trans.Commit();
                            return r;
                        }
                    });

            if (guid.Equals(Guid.Empty)) 
                _logger.Info("No provider agreements found.");
            return guid;
        }
    }
}