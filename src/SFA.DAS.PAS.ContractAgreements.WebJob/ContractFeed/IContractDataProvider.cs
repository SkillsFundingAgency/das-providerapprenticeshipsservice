using System;
using System.Collections.Generic;

using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed
{
    public interface IContractDataProvider
    {
        void ReadEvents(int mostRecentPageNumber, Guid lastBookmarkedItemId, Action<int, IEnumerable<ContractFeedEvent>> action);
    }
}