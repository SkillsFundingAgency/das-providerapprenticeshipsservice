using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUser
{
    public class GetUserQuery : IAsyncRequest<GetUserResponse>
    {
        public string UserRef { get; set; }
    }
}