using MediatR;
using SFA.DAS.ProviderRelationships.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission
{
    public class GetProviderHasRelationshipWithPermissionQueryRequest : IRequest<GetProviderHasRelationshipWithPermissionQueryResponse>
    {
        public PermissionEnumDto Permission { get; set; }
        public long ProviderId { get; set; }
    }
}
