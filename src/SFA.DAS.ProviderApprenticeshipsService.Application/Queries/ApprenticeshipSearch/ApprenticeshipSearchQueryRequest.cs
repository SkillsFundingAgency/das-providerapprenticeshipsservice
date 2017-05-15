using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch
{
    public class ApprenticeshipSearchQueryRequest : IAsyncRequest<ApprenticeshipSearchQueryResponse>
    {
        public long ProviderId { get; set; }
        public ApprenticeshipSearchQuery Query { get; set; }
    }
}
