using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests.MockClasses
{
    public sealed class InMemoryProviderAgreementStatusRepository : IProviderAgreementStatusRepository
    {
        private readonly ILog _logger;

        public readonly List<ContractFeedEvent> Data;
        public int LastPageRead;

        public InMemoryProviderAgreementStatusRepository(ILog logger)
        {
            _logger = logger;
            Data = new List<ContractFeedEvent>();
        }

        public Task AddContractEvent(ContractFeedEvent contractFeedEvent)
        {
            _logger.Info($"Storing event: {contractFeedEvent.Id}");
            Data.Add(contractFeedEvent);
            return Task.FromResult(0);
        }

        public Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId)
        {
            return Task.FromResult(Data.Where(e => e.ProviderId == providerId));
        }

        public Task<ContractFeedEvent> GetMostRecentContractFeedEvent()
        {
            if (Data.Count == 0) return Task.FromResult((ContractFeedEvent)null);

            return Task.FromResult(Data.OrderByDescending(e => e.Updated).First());
        }

        public Task<int> GetMostRecentPageNumber()
        {
            return Task.FromResult(LastPageRead);
        }

        public Task SaveLastRun(EventRun lastRun)
        {
            LastPageRead = lastRun.NewLastReadPageNumber;
            return Task.Delay(1);
        }
    }
}
