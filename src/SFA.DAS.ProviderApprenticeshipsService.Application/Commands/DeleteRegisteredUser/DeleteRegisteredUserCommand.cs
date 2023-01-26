using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser
{
    public class DeleteRegisteredUserCommand: IRequest<Unit>
    {
        public string UserRef { get; set; }
    }
}
