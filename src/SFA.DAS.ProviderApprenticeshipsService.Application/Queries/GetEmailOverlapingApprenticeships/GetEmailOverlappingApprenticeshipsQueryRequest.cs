using System.Collections.Generic;

using MediatR;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmailOverlapingApprenticeships
{
    public class GetEmailOverlappingApprenticeshipsQueryRequest : IRequest<GetEmailOverlappingApprenticeshipsQueryResponse>
    {
        public IEnumerable<Apprenticeship> Apprenticeship { get; set; }
    }
}
