using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission
{
    public class GetProviderRelationshipsWithPermissionQueryHandler : IRequestHandler<
        GetProviderRelationshipsWithPermissionQueryRequest, GetProviderRelationshipsWithPermissionQueryResponse>
    {
        private readonly IProviderRelationshipsApiClient _providerRelationshipsApiClient;

        public GetProviderRelationshipsWithPermissionQueryHandler(IProviderRelationshipsApiClient providerRelationshipsApiClient)
        {
            _providerRelationshipsApiClient = providerRelationshipsApiClient;
        }

        public async Task<GetProviderRelationshipsWithPermissionQueryResponse> Handle(GetProviderRelationshipsWithPermissionQueryRequest request, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new GetProviderRelationshipsWithPermissionQueryResponse
                {
                    ProviderRelationships = new List<ProviderRelationshipResponse.ProviderRelationship>()
                };
            }

            var result = await _providerRelationshipsApiClient.ListRelationshipsWithPermission(new ProviderRelationshipRequest
            {
                Permission = request.Permission,
                Ukprn = request.ProviderId
            });

            return new GetProviderRelationshipsWithPermissionQueryResponse
            {
                ProviderRelationships = result.ProviderRelationships
            };
        }
    }
}
