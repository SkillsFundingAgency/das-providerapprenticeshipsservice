using MediatR;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission
{
    public class GetProviderRelationshipsWithPermissionQueryRequest : IRequest<GetProviderRelationshipsWithPermissionQueryResponse>
    {
        public Operation Permission { get; set; }
        public long ProviderId { get; set; }
    }
}
