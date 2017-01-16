using System;
using System.Collections.Generic;

using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed
{
    public interface IContractDataProvider
    {
        void ReadEvents(Guid lastBookmarkedItemId, Action<int, IEnumerable<ContractFeedEvent>> pageHandler);
    }
}