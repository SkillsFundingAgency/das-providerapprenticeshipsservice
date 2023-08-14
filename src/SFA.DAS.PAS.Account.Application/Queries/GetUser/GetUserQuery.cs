using MediatR;

namespace SFA.DAS.PAS.Account.Application.Queries.GetUser;

public class GetUserQuery : IRequest<GetUserQueryResponse>
{
    public string? UserRef { get; set; }
}