using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUser;

public class GetUserQuery : IRequest<GetUserResponse>
{
    public string UserRef { get; set; }
}