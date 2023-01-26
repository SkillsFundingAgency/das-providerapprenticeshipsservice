using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser
{
    public class DeleteRegisteredUserCommandHandler : IRequestHandler<DeleteRegisteredUserCommand, Unit>
    {
        private readonly IValidator<DeleteRegisteredUserCommand> _validator;
        private readonly IUserRepository _userRepository;

        public DeleteRegisteredUserCommandHandler(IValidator<DeleteRegisteredUserCommand> validator, IUserRepository userRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(DeleteRegisteredUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _userRepository.DeleteUser(request.UserRef);

            return Unit.Value;
        }
    }
}
