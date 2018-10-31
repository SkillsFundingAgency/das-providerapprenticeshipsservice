using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class StubProviderRelationshipsApiClient : IProviderRelationshipsApiClient
    {
        private readonly List<ProviderRelationshipResponse.ProviderRelationship> _relationships;

        public StubProviderRelationshipsApiClient()
        {
            _relationships = new List<ProviderRelationshipResponse.ProviderRelationship>
            {
                new ProviderRelationshipResponse.ProviderRelationship
                {
                    Ukprn = 10005077,
                    EmployerAccountId = 1516,
                    EmployerAccountLegalEntityName = "SAINSBURY'S LIMITED",
                    EmployerAccountLegalEntityPublicHashedId = "DY3GKY",
                    EmployerName = "SAINSBURY'S LIMITED"

                }
            };
        }

        public Task<bool> HasRelationshipWithPermission(ProviderRelationshipRequest request)
        {
            return Task.Run(() => _relationships.Any(r => r.Ukprn == request.Ukprn));
        }

        public Task<bool> HasRelationshipWithPermission(ProviderRelationshipRequest request, CancellationToken token) => HasRelationshipWithPermission(request);

        public Task<ProviderRelationshipResponse> ListRelationshipsWithPermission(ProviderRelationshipRequest request)
        {
            return Task.Run(() =>  new ProviderRelationshipResponse { ProviderRelationships = _relationships });
        }
    }
}
