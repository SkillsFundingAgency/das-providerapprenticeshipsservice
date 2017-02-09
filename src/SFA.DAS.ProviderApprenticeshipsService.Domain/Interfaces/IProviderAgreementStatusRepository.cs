using System.Threading.Tasks;

using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IProviderAgreementStatusRepository
    {
        Task AddContractEvent(ContractFeedEvent contractFeedEvent);

        Task<ContractFeedEvent> GetMostRecentContractFeedEvent();

        Task<int> GetMostRecentPageNumber();

        Task SaveLastRun(EventRun lastRun);
    }
}