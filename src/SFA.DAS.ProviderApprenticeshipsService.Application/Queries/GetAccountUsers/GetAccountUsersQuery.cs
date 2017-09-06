using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAccountUsers
{
    public class GetAccountUsersQuery : IAsyncRequest<GetAccountUsersResponse>
    {
        public long Ukprn { get; set; }
    }
}