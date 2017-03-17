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

            var latestBookmark = await _repository.GetLatestBookmark();
            var pageToReadUri = ""; // TODO: LWA Get url of page with bookmark

            // TODO: LWA - Add check to see if any contracts in the database and log if there isn't
            //if (latestBookmark == null && nextPageNumberToRead > 1)
            //{
            //    _logger.Warn($"The database doesn't currently contain any valid contracts from the feed after {lastFullPageNumberRead} pages have been read.");
            //}

            _logger.Info($"Bookmark: {latestBookmark?.ToString() ?? "{Not set}"}, Latest page read uri: {pageToReadUri}");

            var insertedEvents = _dataProvider.ReadEvents(pageToReadUri, latestBookmark, async (eventPageNumber, events) =>
                {
                    var contractFeedEvents = events.ToList();

                    await _repository.AddContractEventsForPage(eventPageNumber, contractFeedEvents);
                });

            if (insertedEvents > 0)
            {
                _logger.Info($"Inserted {insertedEvents} contracts into the database.");
            }

            time.Stop();

            _logger.Info($"Run took {time.ElapsedMilliseconds} milliseconds.");
        }
    }

    public interface IProviderAgreementStatusService
    {
        Task UpdateProviderAgreementStatuses();
    }
}
