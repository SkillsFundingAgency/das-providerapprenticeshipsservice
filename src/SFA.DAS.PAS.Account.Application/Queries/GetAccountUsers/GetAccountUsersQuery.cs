using MediatR;

namespace SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;

public class GetAccountUsersQuery : IRequest<GetAccountUsersResponse>
{
    public long Ukprn { get; set; }
}