using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IProviderAgreementStatusRepository
    {
        Task AddContractEvent(ContractFeedEvent contractFeedEvent);

        Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId);

        Task<Guid> GetMostRecentBookmarkId();
    }
}