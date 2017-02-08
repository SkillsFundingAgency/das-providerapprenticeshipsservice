using System;
using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.ContractAgreements.WebJob
{
    public class ProviderAgreementStatusService : IProviderAgreementStatusService
    {
        private readonly IContractDataProvider _dataProvider;
        private readonly IProviderAgreementStatusRepository _repository;

        private readonly ILog _logger;

        public ProviderAgreementStatusService(
            IContractDataProvider dataProvider, 
            IProviderAgreementStatusRepository repository,
            ILog logger)
        {
            _dataProvider = dataProvider;
            _repository = repository;
            _logger = logger;
        }

        public async Task UpdateProviderAgreementStatuses()
        {
            var lastContact = await _repository.GetMostRecentContractFeedEvent();
            var mostRecentPageNumber = await _repository.GetMostRecentPageNumber();

            // if last contact have a page number there might be more records on that page we have not read yet.
            if (lastContact?.PageNumber > 0) mostRecentPageNumber = lastContact.PageNumber;
            else ++mostRecentPageNumber;

            _logger.Info($"Bookmark: {lastContact?.Id}, Latest page: {mostRecentPageNumber}");

            var lastRun = _dataProvider.ReadEvents(mostRecentPageNumber, lastContact?.Id ?? Guid.Empty, (eventPageNumber, events) =>
                {
                    var contractFeedEvents = events.ToList();

                    foreach (var contractFeedEvent in contractFeedEvents)
                    {
                        contractFeedEvent.PageNumber = eventPageNumber;
                        _repository.AddContractEvent(contractFeedEvent);
                    }
                });

            _repository.SaveLastRun(lastRun);
            // write  readStatistics  to database
            // [] DB queries
            // []
        }
    }

    public interface IProviderAgreementStatusService
    {
        Task UpdateProviderAgreementStatuses();
    }
}
