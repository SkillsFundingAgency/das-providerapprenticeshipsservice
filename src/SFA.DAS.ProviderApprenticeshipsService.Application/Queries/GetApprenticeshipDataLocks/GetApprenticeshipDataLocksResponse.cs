using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLocks
{
    public sealed class GetApprenticeshipDataLocksResponse
    {
        public List<Commitments.Api.Types.DataLock.DataLockStatus> Data { get; set; }
    }
}