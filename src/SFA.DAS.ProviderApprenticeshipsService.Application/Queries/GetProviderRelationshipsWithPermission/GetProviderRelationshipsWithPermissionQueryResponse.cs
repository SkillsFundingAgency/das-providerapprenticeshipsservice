using System.Collections.Generic;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission
{
    public class GetProviderRelationshipsWithPermissionQueryResponse
    {
        public IEnumerable<AccountProviderLegalEntityDto> ProviderRelationships { get; set; }
    }
}
