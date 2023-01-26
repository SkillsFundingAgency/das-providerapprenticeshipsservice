using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandHandler: IRequestHandler<UpsertRegisteredUserCommand, Unit>
    {
        private readonly IValidator<UpsertRegisteredUserCommand> _validator;
        private readonly IUserRepository _userRepository;

        public UpsertRegisteredUserCommandHandler(
            IValidator<UpsertRegisteredUserCommand> validator,
            IUserRepository userRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(UpsertRegisteredUserCommand message, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _userRepository.Upsert(new User
            {
                UserRef = message.UserRef,
                DisplayName = message.DisplayName,
                Email = message.Email,
                Ukprn = message.Ukprn,
                IsDeleted = false
            });

            return Unit.Value;
        }
    }
}
