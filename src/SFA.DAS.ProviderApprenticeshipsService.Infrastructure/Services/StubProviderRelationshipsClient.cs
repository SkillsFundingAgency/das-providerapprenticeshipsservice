using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class StubProviderRelationshipsApiClient : IProviderRelationshipsApiClient
    {
        private readonly List<RelationshipDto> _relationships;

        public StubProviderRelationshipsApiClient()
        {
            _relationships = new List<RelationshipDto>
            {
                new RelationshipDto
                {
                    Ukprn = 10005077,
                    EmployerAccountId = 1516,
                    EmployerAccountLegalEntityName = "SAINSBURY'S LIMITED",
                    EmployerAccountLegalEntityPublicHashedId = "DY3GKY",
                    EmployerAccountName = "SAINSBURY'S LIMITED"
                }
            };
        }

        public Task<bool> HasPermission(PermissionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Run(() => _relationships.Any(r => r.Ukprn == request.Ukprn), cancellationToken);
        }

        public Task<bool> HasRelationshipWithPermission(RelationshipsRequest request, CancellationToken token) => HasRelationshipWithPermission(request, token);

        public Task<RelationshipsResponse> GetRelationshipsWithPermission(RelationshipsRequest request, CancellationToken token)
        {
            return Task.Run(() =>  new RelationshipsResponse { Relationships = _relationships }, token);
        }
    }
}
