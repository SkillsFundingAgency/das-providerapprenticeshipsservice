using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary
{
    public class GetApprenticeshipDataLockSummaryQueryResponse
    {
        public DataLockSummary DataLockSummary { get; set; }
    }
}
