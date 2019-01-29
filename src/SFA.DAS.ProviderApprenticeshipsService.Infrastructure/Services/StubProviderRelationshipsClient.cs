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

        public StubProviderRelationshipsApiClient(IPublicHashingService publicHashingService, IAccountLegalEntityPublicHashingService accountLegalEntityPublicHashingService)
        {

            const long stubAccountId = 103380;
            const long stubAccountLegalEntityId = 36637;

            _accountProviderLegalEntitiesByUkprn = new List<KeyValuePair<long, AccountProviderLegalEntityDto>>
            {
                new KeyValuePair<long, AccountProviderLegalEntityDto>(10005077, new AccountProviderLegalEntityDto
                {
                    AccountId = stubAccountId,
                    AccountPublicHashedId = publicHashingService.HashValue(stubAccountId),
                    AccountLegalEntityName = "TEST LEGAL ENTITY ACCOUNT",
                    AccountLegalEntityId = stubAccountLegalEntityId,
                    AccountLegalEntityPublicHashedId = "GEGZK5", //accountLegalEntityPublicHashingService.HashValue(stubAccountLegalEntityId),
                    AccountName = "TEST (ACCOUNT)"
                }),
                new KeyValuePair<long, AccountProviderLegalEntityDto>(10005077, new AccountProviderLegalEntityDto
                {
                    AccountId = 8194,
                    AccountPublicHashedId = "9V6JWR",
                    AccountLegalEntityName = "ASAP CATERING LIMITED (Stub)",
                    AccountLegalEntityId = 3884,
                    AccountLegalEntityPublicHashedId = "94DVK9",
                    AccountName = "ASAP CATERING LIMITED (Stub) (ACCOUNT)"
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
