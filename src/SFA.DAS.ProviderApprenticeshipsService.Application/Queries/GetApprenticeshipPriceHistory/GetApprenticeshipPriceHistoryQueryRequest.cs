using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory
{
    public class GetApprenticeshipPriceHistoryQueryRequest: IAsyncRequest<GetApprenticeshipPriceHistoryQueryResponse>
    {
        public long ApprenticeshipId { get; set; }
    }
}
