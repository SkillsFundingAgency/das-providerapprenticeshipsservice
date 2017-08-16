using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUser
{
    public class GetUserRequest : IAsyncRequest<GetUserResponse>
    {
        public string UserRef { get; set; }
    }
}
