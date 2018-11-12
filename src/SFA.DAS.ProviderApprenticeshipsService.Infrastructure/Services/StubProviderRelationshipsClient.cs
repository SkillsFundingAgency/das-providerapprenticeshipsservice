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
        private readonly IPublicHashingService _publicHashingService;
        private readonly List<RelationshipDto> _relationships;

        public StubProviderRelationshipsApiClient(IPublicHashingService publicHashingService)
        {
            _publicHashingService = publicHashingService;
            _relationships = new List<RelationshipDto>
            {
                new RelationshipDto
                {
                    Ukprn = 10005077,
                    EmployerAccountId = 1516,
                    EmployerAccountPublicHashedId = _publicHashingService.HashValue(1516),
                    EmployerAccountLegalEntityName = "SAINSBURY'S LIMITED",
                    EmployerAccountLegalEntityId = 353,
                    EmployerAccountLegalEntityPublicHashedId = _publicHashingService.HashValue(353),
                    EmployerAccountName = "SAINSBURY'S LIMITED"
                }
            };
        }

        public Task<bool> HasPermission(PermissionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Run(() => _relationships.Any(r => r.Ukprn == request.Ukprn), cancellationToken);
        }

        public async Task<bool> HasRelationshipWithPermission(RelationshipsRequest request, CancellationToken token)
        {
            return (await GetRelationshipsWithPermission(request, token)).Relationships.Any();
        }

        public Task<RelationshipsResponse> GetRelationshipsWithPermission(RelationshipsRequest request, CancellationToken token)
        {
            return Task.Run(() =>  new RelationshipsResponse { Relationships = _relationships }, token);
        }
    }
}
