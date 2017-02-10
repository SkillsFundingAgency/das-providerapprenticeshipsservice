using System.Collections.Generic;
using System.Threading.Tasks;

using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Data
{
    public interface IAgreementStatusQueryRepository
    {
        Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId);
    }
}
