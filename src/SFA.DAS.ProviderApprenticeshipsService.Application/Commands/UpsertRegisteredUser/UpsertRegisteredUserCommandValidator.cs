using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandValidator : AbstractValidator<UpsertRegisteredUserCommand>
    {
        public UpsertRegisteredUserCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty();
            RuleFor(x => x.Ukprn).NotEmpty();
        }
    }
}
