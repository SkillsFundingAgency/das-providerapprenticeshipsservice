using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IProviderAgreementStatusRepository
    {
        void AddContractEvent(ContractFeedEvent contractFeedEvent);

        IEnumerable<ContractFeedEvent> GetContractEvents(long providerId);

        Guid GetMostRecentBookmarkId();
    }
}