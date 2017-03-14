using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships
{
    public class GetOverlappingApprenticeshipsQueryResponse
    {
        public IEnumerable<OverlapApprenticeship> Overlaps { get; set; }
    }

    public class OverlapApprenticeship
    {
        public long EmployerAccountId { get; set; }

        public long ProviderId { get; set; }

        public string ProviderName { get; set; }

        public string LegalEntityName { get; set; }

        public string ValidationFailReason { get; set; }
    }

}