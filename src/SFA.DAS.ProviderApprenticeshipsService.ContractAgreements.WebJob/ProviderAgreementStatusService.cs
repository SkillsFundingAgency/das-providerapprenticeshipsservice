using System;
using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob
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

        public async Task<ProviderAgreementStatus> GetProviderAgreementStatus(long providerId)
        {
            var providersEvents = await _repository.GetContractEvents(providerId);

            return providersEvents.Any(m => m.Status.Equals("approved", StringComparison.CurrentCultureIgnoreCase)) 
                ? ProviderAgreementStatus.Agreed : ProviderAgreementStatus.NotAgreed;
        }

        public async Task UpdateProviderAgreementStatuses()
        {
            var lastBookmarkedItemId = await _repository.GetMostRecentBookmarkId();

            _logger.Info($"Bookmark: {lastBookmarkedItemId}");

            _dataProvider.ReadEvents(lastBookmarkedItemId, (pageNumber, events) =>
            {
                var contractFeedEvents = events.ToList();

                foreach (var contractFeedEvent in contractFeedEvents)
                {
                    _repository.AddContractEvent(contractFeedEvent);
                }
            });
        }
    }

    public interface IProviderAgreementStatusService
    {
        Task<ProviderAgreementStatus> GetProviderAgreementStatus(long providerId);

        Task UpdateProviderAgreementStatuses();
    }
}
