using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory
{
    public class GetApprenticeshipPriceHistoryQueryResponse
    {
        public List<PriceHistory> History { get; set; }
    }
}
