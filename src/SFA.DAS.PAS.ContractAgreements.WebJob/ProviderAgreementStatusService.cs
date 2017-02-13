using System;
using System.Diagnostics;
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
            var time = new Stopwatch();
            time.Start();

            var lastContract = await _repository.GetMostRecentContractFeedEvent();
            var lastFullPageNumberRead = await _repository.GetMostRecentPageNumber();
            var nextPageNumberToRead = lastFullPageNumberRead + 1;

            if (lastContract == null && nextPageNumberToRead > 1)
            {
                _logger.Warn($"The database doesn't currently contain any valid contracts from the feed after {lastFullPageNumberRead} pages have been read.");
            }

            _logger.Info($"Bookmark: {lastContract?.Id}, Latest page read: {lastFullPageNumberRead}");

            var lastRun = _dataProvider.ReadEvents(nextPageNumberToRead, lastContract?.Id ?? Guid.Empty, (eventPageNumber, events) =>
                {
                    var contractFeedEvents = events.ToList();

                    foreach (var contractFeedEvent in contractFeedEvents)
                    {
                        contractFeedEvent.PageNumber = eventPageNumber;
                        _repository.AddContractEvent(contractFeedEvent);
                    }
                });

            if (lastRun.ContractCount > 0)
            {
                _logger.Info($"Inserted {lastRun.ContractCount} contracts into the database. LastPageRead: {lastRun.NewLastReadPageNumber}");
            }

            time.Stop();
            lastRun.ExecutionTimeMs = time.ElapsedMilliseconds;
            await _repository.SaveLastRun(lastRun);
        }
    }

    public interface IProviderAgreementStatusService
    {
        Task UpdateProviderAgreementStatuses();
    }
}
