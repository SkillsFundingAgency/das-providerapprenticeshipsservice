using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmployerAccountLegalEntities
{
    public class GetEmployerAccountLegalEntitiesHandler : IRequestHandler<GetEmployerAccountLegalEntitiesRequest, GetEmployerAccountLegalEntitiesResponse>
    {
        private readonly IEmployerAccountService _employerAccountService;

        public GetEmployerAccountLegalEntitiesHandler(IEmployerAccountService employerAccountService)
        {
            _employerAccountService = employerAccountService;
        }

        public async Task<GetEmployerAccountLegalEntitiesResponse> Handle(GetEmployerAccountLegalEntitiesRequest request, CancellationToken cancellationToken)
        {
            var legalEntities = await _employerAccountService.GetLegalEntitiesForAccount(request.HashedAccountId);

            return new GetEmployerAccountLegalEntitiesResponse { LegalEntities = legalEntities };
        }
    }
}
