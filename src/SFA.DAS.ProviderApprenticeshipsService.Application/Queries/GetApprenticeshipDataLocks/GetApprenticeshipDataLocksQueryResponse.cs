using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLocks
{
    public class GetApprenticeshipDataLocksQueryResponse
    {
        public List<DataLockStatus> DataLockSummary { get; set; }
    }
}
