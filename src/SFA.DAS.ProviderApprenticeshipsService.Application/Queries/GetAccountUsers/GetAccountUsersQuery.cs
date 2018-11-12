using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAccountUsers
{
    public class GetAccountUsersQuery : IRequest<GetAccountUsersResponse>
    {
        public long Ukprn { get; set; }
    }
}