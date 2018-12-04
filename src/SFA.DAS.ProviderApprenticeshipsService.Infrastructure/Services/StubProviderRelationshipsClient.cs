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
        private readonly List<KeyValuePair<long /*ukprn*/, AccountProviderLegalEntityDto>> _accountProviderLegalEntitiesByUkprn;

        public StubProviderRelationshipsApiClient()
        {
            _accountProviderLegalEntitiesByUkprn = new List<KeyValuePair<long, AccountProviderLegalEntityDto>>
            {
                new KeyValuePair<long, AccountProviderLegalEntityDto>(10005077, new AccountProviderLegalEntityDto
                {
                    AccountId = 15623,
                    AccountPublicHashedId = "74449P",
                    AccountLegalEntityName = "RED DIAMONDS",
                    AccountLegalEntityId = 8004,
                    AccountLegalEntityPublicHashedId = "XPBMMX",
                    AccountName = "RED DIAMONDS (ACCOUNT)"
                })
            };
        }

        public Task<GetAccountProviderLegalEntitiesWithPermissionResponse> GetAccountProviderLegalEntitiesWithPermission(
            GetAccountProviderLegalEntitiesWithPermissionRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(new GetAccountProviderLegalEntitiesWithPermissionResponse
                { AccountProviderLegalEntities = _accountProviderLegalEntitiesByUkprn.Where(kvp => kvp.Key == request.Ukprn).Select(kvp => kvp.Value) });
        }

        public Task<bool> HasPermission(HasPermissionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(_accountProviderLegalEntitiesByUkprn.Any(r => r.Key == request.Ukprn));
        }

        public async Task<bool> HasRelationshipWithPermission(HasRelationshipWithPermissionRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return (await GetAccountProviderLegalEntitiesWithPermission(new GetAccountProviderLegalEntitiesWithPermissionRequest
                { Ukprn = request.Ukprn, Operation = request.Operation }, cancellationToken)).AccountProviderLegalEntities.Any();
        }

        public Task HealthCheck()
        {
            throw new System.NotImplementedException();
        }
    }
}
