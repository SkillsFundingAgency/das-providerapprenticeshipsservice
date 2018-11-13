using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission
{
    public class GetProviderHasRelationshipWithPermissionQueryHandler : IRequestHandler<
        GetProviderHasRelationshipWithPermissionQueryRequest, GetProviderHasRelationshipWithPermissionQueryResponse>
    {
        private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;

        public GetProviderHasRelationshipWithPermissionQueryHandler(IProviderRelationshipsApiClient providerRelationshipsApiClient)
        {
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
        }

        public async Task<GetProviderHasRelationshipWithPermissionQueryResponse> Handle(
            GetProviderHasRelationshipWithPermissionQueryRequest request,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new GetProviderHasRelationshipWithPermissionQueryResponse();
            }

            var apiRequest = new ProviderRelationshipRequest
            {
                Permission = request.Permission,
                Ukprn = request.ProviderId
            };

            var result = await _providerRelationshipsApiClient.HasRelationshipWithPermission(apiRequest, cancellationToken);

            return new GetProviderHasRelationshipWithPermissionQueryResponse
            {
                HasPermission = result
            };
        }
    }
}
