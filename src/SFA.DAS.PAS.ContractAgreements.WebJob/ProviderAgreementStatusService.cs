using System.Diagnostics;
using System.Threading.Tasks;
using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.PAS.ContractAgreements.WebJob
{
    public sealed class ProviderAgreementStatusService : IProviderAgreementStatusService
    {
        private readonly IContractDataProvider _dataProvider;
        private readonly IProviderAgreementStatusRepository _repository;
        private readonly ILogger<ProviderAgreementStatusService> _logger;

        public ProviderAgreementStatusService(
            IContractDataProvider dataProvider, 
            IProviderAgreementStatusRepository repository,
            ILogger<ProviderAgreementStatusService> logger)
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
            
            _logger.LogInformation($"Last bookmark: {latestBookmark?.ToString() ?? "[not set]"}, Next page to read url: {pageToReadUrl}");

            var insertedEvents = _dataProvider.ReadEvents(pageToReadUrl, latestBookmark, (events, newBookmark) =>
                {
                    _repository.AddContractEventsForPage(events, newBookmark.Value).Wait();
                });

            if (insertedEvents > 0)
            {
                _logger.LogInformation($"Inserted {insertedEvents} contracts into the database.");
            }

            time.Stop();

            _logger.LogInformation($"Run took {time.ElapsedMilliseconds} milliseconds.");

            if (await _repository.GetCountOfContracts() == 0)
            {
                _logger.LogWarning($"The database doesn't currently contain any valid contracts from the feed.");
            }
        }
    }
}
