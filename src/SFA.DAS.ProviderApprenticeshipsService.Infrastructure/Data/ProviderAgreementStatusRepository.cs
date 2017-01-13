using System;
using System.Collections.Generic;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data
{
    public class ProviderAgreementStatusRepository : IProviderAgreementStatusRepository
    {
        private readonly ILog _logger;

        private readonly List<ContractFeedEvent> _data;

        public ProviderAgreementStatusRepository(ILog logger)
        {
            _logger = logger;
            _data = new List<ContractFeedEvent>();
        }

        public void AddContractEvent(ContractFeedEvent contractFeedEvent)
        {
            throw new NotImplementedException($"Not implemented exception: {this.GetType()}");
            //_logger.Info($"Storing event: {contractFeedEvent.Id}");
            //_data.Add(contractFeedEvent);
        }

        public IEnumerable<ContractFeedEvent> GetContractEvents(long providerId)
        {
            throw new NotImplementedException($"Not implemented exception: {this.GetType()}");
            //return _data.Where(e => e.ProviderId == providerId);
        }

        public Guid GetMostRecentBookmarkId()
        {
            throw new NotImplementedException($"Not implemented exception: {this.GetType()}");
            //if (_data.Count == 0) return Guid.Empty;

            //return _data.OrderByDescending(e => e.Updated).First().Id;
        }
    }
}