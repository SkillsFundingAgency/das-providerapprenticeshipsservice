using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests.MockClasses
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

        public Task AddContractEvent(ContractFeedEvent contractFeedEvent)
        {
            _logger.Info($"Storing event: {contractFeedEvent.Id}");
            _data.Add(contractFeedEvent);
            return Task.FromResult(0);
        }

        public async Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId)
        {
            return _data.Where(e => e.ProviderId == providerId);
        }

        public async Task<ContractFeedEvent> GetMostRecentContractFeedEvent()
        {
            if (_data.Count == 0) return null;

            return _data.OrderByDescending(e => e.Updated).First();
        }

        public Task<int> GetMostRecentPageNumber()
        {
            if (_data.Count == 0) return Task.FromResult(0);
            var pn = _data
                .Where(m => m.PageNumber > 0)
                .OrderByDescending(e => e.Updated).First().PageNumber;
            return Task.FromResult(pn);
        }

        public Task SaveLastRun(EventRun lastRun)
        {
            throw new System.NotImplementedException();
        }
    }
}
