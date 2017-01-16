using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.UnitTests.MockClasses
{
    public class InMemoryProviderAgreementStatusRepository : IProviderAgreementStatusRepository
    {
        private readonly ILog _logger;

        private readonly List<ContractFeedEvent> _data;

        public InMemoryProviderAgreementStatusRepository(ILog logger)
        {
            _logger = logger;
            _data = new List<ContractFeedEvent>();
        }

        public void AddContractEvent(ContractFeedEvent contractFeedEvent)
        {
            _logger.Info($"Storing event: {contractFeedEvent.Id}");
            _data.Add(contractFeedEvent);
        }

        public async Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId)
        {
            return _data.Where(e => e.ProviderId == providerId);
        }

        public async Task<Guid> GetMostRecentBookmarkId()
        {
            if (_data.Count == 0) return Guid.Empty;

            return _data.OrderByDescending(e => e.Updated).First().Id;
        }

        public Task SaveContractEvents()
        {
            return Task.FromResult(0);
        }
    }
}
