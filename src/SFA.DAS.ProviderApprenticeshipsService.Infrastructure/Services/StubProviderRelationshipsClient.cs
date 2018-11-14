using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class StubProviderRelationshipsApiClient : IProviderRelationshipsApiClient
    {
        private readonly IPublicHashingService _publicHashingService;
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public StubProviderRelationshipsApiClient(IPublicHashingService publicHashingService, IProviderCommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
            _publicHashingService = publicHashingService;
        }

        private async Task<List<RelationshipDto>> GetStubRelationships()
        {
            var apprenticeships = await  _commitmentsApi.GetProviderApprenticeships(10005077);
            
            var result = new List<RelationshipDto>();
            foreach (var apprenticeship in apprenticeships)
            {

                var relationship = new RelationshipDto
                {
                    EmployerAccountLegalEntityName = apprenticeship.LegalEntityName,
                    EmployerAccountId = apprenticeship.EmployerAccountId,
                    EmployerAccountLegalEntityId =
                        _publicHashingService.DecodeValue(apprenticeship.AccountLegalEntityPublicHashedId),
                    EmployerAccountLegalEntityPublicHashedId = apprenticeship.AccountLegalEntityPublicHashedId,
                    EmployerAccountName = apprenticeship.LegalEntityName + " (Account)",
                    EmployerAccountProviderId = 10005077,
                    EmployerAccountPublicHashedId = _publicHashingService.HashValue(apprenticeship.EmployerAccountId),
                    Ukprn = 10005077
                };

                if (!result.Any(x =>
                    x.EmployerAccountId == relationship.EmployerAccountId && x.EmployerAccountLegalEntityId ==
                    relationship.EmployerAccountLegalEntityId))
                {
                    result.Add(relationship);
                }
            }

            return result;

        }

        public Task<bool> HasPermission(PermissionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Run(() => GetStubRelationships().Result.Any(r => r.Ukprn == request.Ukprn), cancellationToken);
        }

        public async Task<bool> HasRelationshipWithPermission(RelationshipsRequest request, CancellationToken token)
        {
            return (await GetRelationshipsWithPermission(request, token)).Relationships.Any();
        }

        public Task<RelationshipsResponse> GetRelationshipsWithPermission(RelationshipsRequest request, CancellationToken token)
        {
            return Task.Run(() =>  new RelationshipsResponse { Relationships = GetStubRelationships().Result }, token);
        }
    }
}
