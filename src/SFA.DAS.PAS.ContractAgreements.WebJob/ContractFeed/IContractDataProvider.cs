using System;
using System.Collections.Generic;

using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed
{
    public interface IContractDataProvider
    {
        EventRun ReadEvents(int mostRecentPageNumber, Guid lastBookmarkedItemId, Action<int, IEnumerable<ContractFeedEvent>> action);
    }
}