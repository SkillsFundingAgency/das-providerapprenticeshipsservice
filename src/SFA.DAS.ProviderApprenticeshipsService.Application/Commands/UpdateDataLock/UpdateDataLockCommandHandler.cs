using System.Threading.Tasks;

using FluentValidation;

using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.RequestApprenticeshipRestart;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateDataLock
{
    public sealed class UpdateDataLockCommandHandler : AsyncRequestHandler<UpdateDataLockCommand>
    {
        private readonly AbstractValidator<UpdateDataLockCommand> _validator;

        public UpdateDataLockCommandHandler(
            AbstractValidator<UpdateDataLockCommand> validator)
        {
            _validator = validator;
        }

        protected override async Task HandleCore(UpdateDataLockCommand command)
        {
            _validator.ValidateAndThrow(command);
            // ToDo: Call update method .. message.TriageStatus
            //throw new System.NotImplementedException();
        }
    }
}