using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IAgreementStatusQueryRepository
    {
        Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId);
    }
}
