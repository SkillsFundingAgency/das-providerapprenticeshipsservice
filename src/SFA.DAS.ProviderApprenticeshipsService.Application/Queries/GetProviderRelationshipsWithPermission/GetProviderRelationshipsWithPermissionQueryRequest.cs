using MediatR;
using SFA.DAS.ProviderRelationships.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission
{
    public class GetProviderRelationshipsWithPermissionQueryRequest : IRequest<GetProviderRelationshipsWithPermissionQueryResponse>
    {
        public PermissionEnumDto Permission { get; set; }
        public long ProviderId { get; set; }
    }
}
