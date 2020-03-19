using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser
{
    public class DeleteRegisteredUserCommandValidator : AbstractValidator<DeleteRegisteredUserCommand>
    {
        public DeleteRegisteredUserCommandValidator()
        {
            RuleFor(x => x.UserRef).NotEmpty();
        }
    }
}
