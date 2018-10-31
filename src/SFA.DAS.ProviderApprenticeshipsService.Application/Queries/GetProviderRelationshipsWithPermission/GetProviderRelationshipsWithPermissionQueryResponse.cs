using System.Collections.Generic;
using SFA.DAS.ProviderRelationships.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission
{
    public class GetProviderRelationshipsWithPermissionQueryResponse
    {
        public IEnumerable<ProviderRelationshipResponse.ProviderRelationship> ProviderRelationships { get; set; }
    }
}
