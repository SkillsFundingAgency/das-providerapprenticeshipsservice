using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandHandler: AsyncRequestHandler<UpsertRegisteredUserCommand>
    {
        public UpsertRegisteredUserCommandHandler()
        {
            
        }

        protected override Task HandleCore(UpsertRegisteredUserCommand message)
        {
            return Task.FromResult(new Unit());
        }
    }
}
