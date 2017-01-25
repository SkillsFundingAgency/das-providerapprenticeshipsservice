using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IAgreementStatusQueryRepository
    {
        Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId);
    }
}