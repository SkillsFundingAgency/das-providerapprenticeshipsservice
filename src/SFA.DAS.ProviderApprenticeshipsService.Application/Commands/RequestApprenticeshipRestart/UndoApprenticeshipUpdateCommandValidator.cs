using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.RequestApprenticeshipRestart
{
    public sealed class UndoApprenticeshipUpdateCommandValidator :
        AbstractValidator<RestartApprenticeshipCommand>
    {
        public UndoApprenticeshipUpdateCommandValidator()
        {
            RuleFor(x => x.ApprenticeshipId).NotEmpty();
            RuleFor(x => x.ProviderId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}