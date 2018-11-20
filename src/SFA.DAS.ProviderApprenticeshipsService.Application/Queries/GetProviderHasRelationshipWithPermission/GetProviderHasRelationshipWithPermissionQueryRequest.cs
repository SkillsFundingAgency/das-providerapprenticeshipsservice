using MediatR;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission
{
    public class GetProviderHasRelationshipWithPermissionQueryRequest : IRequest<GetProviderHasRelationshipWithPermissionQueryResponse>
    {
        public Operation Permission { get; set; }
        public long ProviderId { get; set; }
    }
}
