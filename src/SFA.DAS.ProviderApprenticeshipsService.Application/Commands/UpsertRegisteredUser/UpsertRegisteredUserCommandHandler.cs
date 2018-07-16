using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandHandler: AsyncRequestHandler<UpsertRegisteredUserCommand>
    {
        private readonly AbstractValidator<UpsertRegisteredUserCommand> _validator;
        private readonly IUserRepository _userRepository;

        public UpsertRegisteredUserCommandHandler(
            AbstractValidator<UpsertRegisteredUserCommand> validator,
            IUserRepository userRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(UpsertRegisteredUserCommand message)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _userRepository.Upsert(new User
            {
                UserRef = message.UserRef,
                DisplayName = message.DisplayName,
                Email = message.Email,
                Ukprn = message.Ukprn
            });
        }
    }
}
