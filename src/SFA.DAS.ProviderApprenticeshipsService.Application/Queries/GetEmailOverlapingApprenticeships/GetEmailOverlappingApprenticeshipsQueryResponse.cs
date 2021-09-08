using SFA.DAS.Commitments.Api.Types.Validation;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmailOverlapingApprenticeships
{
    public class GetEmailOverlappingApprenticeshipsQueryResponse
    {
        public IEnumerable<ApprenticeshipEmailOverlapValidationResult> Overlaps { get; set; }

        public IEnumerable<OverlappingApprenticeship> GetOverlappingApprenticeships(long apprenticeshipId)
        {
            return Overlaps.FirstOrDefault(m => m.Self.ApprenticeshipId == apprenticeshipId)
                ?.OverlappingApprenticeships
                ?? Enumerable.Empty<OverlappingApprenticeship>();
        }

        public IEnumerable<OverlappingApprenticeship> GetFirstOverlappingApprenticeships()
        {
            return Overlaps.FirstOrDefault()?.OverlappingApprenticeships
                ?? Enumerable.Empty<OverlappingApprenticeship>();
        }
    }
}
