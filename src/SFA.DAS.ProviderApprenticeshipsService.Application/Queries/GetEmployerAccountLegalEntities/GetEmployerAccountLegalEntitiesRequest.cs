using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmployerAccountLegalEntities
{
    public class GetEmployerAccountLegalEntitiesRequest : IRequest<GetEmployerAccountLegalEntitiesResponse>
    {
        public string UserId { get; set; }
        public string HashedAccountId { get; set; }
    }
}
