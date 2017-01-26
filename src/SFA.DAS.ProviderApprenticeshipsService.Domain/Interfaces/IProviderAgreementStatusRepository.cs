using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IProviderAgreementStatusRepository
    {
        Task AddContractEvent(ContractFeedEvent contractFeedEvent);

        Task<ContractFeedEvent> GetMostRecentContractFeedEvent();

        Task<int> GetMostRecentPageNumber();
    }
}