using MediatR;

namespace SFA.DAS.PAS.Account.Application.Queries.GetUser
{
    public class GetUserQuery : IRequest<GetUserResponse>
    {
        public string UserRef { get; set; }
    }
}