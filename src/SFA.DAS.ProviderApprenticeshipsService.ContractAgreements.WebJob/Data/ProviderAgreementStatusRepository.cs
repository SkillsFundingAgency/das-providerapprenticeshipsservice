using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.Data
{
    public class ProviderAgreementStatusRepository : DbBaseRepository, IProviderAgreementStatusRepository
    {
        private readonly ILog _logger;

        private List<ContractFeedEvent> _data;

        public ProviderAgreementStatusRepository(ContractFeedConfiguration config, ILog logger) : base (config.DatabaseConnectionString)
        {
            _logger = logger;
            _data = new List<ContractFeedEvent>();
        }

        public void AddContractEvent(ContractFeedEvent contractFeedEvent)
        {
            _data.Add(contractFeedEvent);
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
        }

        public async Task<Guid> GetMostRecentBookmarkId()
        {
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

        public async Task SaveContractEvents()
        {
            _logger.Info($"Saving {_data.Count} contract to database");
            foreach (var contractFeedEvent in _data)
            {
                await AddContractEventPrivate(contractFeedEvent);
            }
            _data = new List<ContractFeedEvent>();
        }

        private async Task AddContractEventPrivate(ContractFeedEvent contractFeedEvent)
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
    }
}