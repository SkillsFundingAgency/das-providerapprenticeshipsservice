using System;
using System.Collections.Generic;

using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed
{
    public interface IContractDataProvider
    {
        string FindPageWithBookmark(Guid? latestBookmark);

        int ReadEvents(string pageToReadUri, Guid? latestBookmark, Action<int, IEnumerable<ContractFeedEvent>> action);
    }
}