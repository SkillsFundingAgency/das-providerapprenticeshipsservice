using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.TriageApprenticeshipDataLocks
{
    public class TriageApprenticeshipDataLocksCommandValidator : AbstractValidator<TriageApprenticeshipDataLocksCommand>
    {
        public TriageApprenticeshipDataLocksCommandValidator()
        {
            RuleFor(x => x.ApprenticeshipId).NotEmpty();
            RuleFor(x => x.TriageStatus).IsInEnum();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProviderId).NotEmpty();
        }
    }
}
