using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;

namespace SFA.DAS.PAS.ContractAgreements.WebJob
{
    public sealed class ProviderAgreementStatusService : IProviderAgreementStatusService
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
            var pageToReadUrl = _dataProvider.FindPageWithBookmark(latestBookmark);
            
            _logger.Info($"Bookmark: {latestBookmark?.ToString() ?? "[Empty]"}, Next page to read url: {pageToReadUrl}");

            var insertedEvents = _dataProvider.ReadEvents(pageToReadUrl, latestBookmark, (events, newBookmark) =>
                {
                    var contractFeedEvents = events.ToList();
                    if (events.Count() > 0)
                    {
                        _repository.AddContractEventsForPage(contractFeedEvents, newBookmark.Value).Wait();
                    }
                });

            if (insertedEvents > 0)
            {
                _logger.Info($"Inserted {insertedEvents} contracts into the database.");
            }

            time.Stop();

            _logger.Info($"Run took {time.ElapsedMilliseconds} milliseconds.");

            if (await _repository.GetCountOfContracts() == 0)
            {
                _logger.Warn($"The database doesn't currently contain any valid contracts from the feed.");
            }
        }
    }
}
