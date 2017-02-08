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
            var mostRecentPageNumber = await _repository.GetMostRecentPageNumber() +1;
            if (lastContract == null) mostRecentPageNumber = 1;

            _logger.Info($"Bookmark: {lastContract?.Id}, Latest page read: {mostRecentPageNumber}");

            var lastRun = _dataProvider.ReadEvents(mostRecentPageNumber, lastContract?.Id ?? Guid.Empty, (eventPageNumber, events) =>
                {
                    var contractFeedEvents = events.ToList();

                    foreach (var contractFeedEvent in contractFeedEvents)
                    {
                        contractFeedEvent.PageNumber = eventPageNumber;
                        _repository.AddContractEvent(contractFeedEvent);
                    }
                });

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
