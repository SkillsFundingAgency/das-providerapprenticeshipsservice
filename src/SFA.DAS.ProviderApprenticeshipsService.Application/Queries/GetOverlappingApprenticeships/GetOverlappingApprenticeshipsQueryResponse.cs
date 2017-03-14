using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships
{
    public class GetOverlappingApprenticeshipsQueryResponse
    {
        public IEnumerable<OverlappingApprenticeship> Overlaps { get; set; }
    }
}