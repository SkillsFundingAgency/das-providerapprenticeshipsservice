using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateDataLock
{
    public sealed class UpdateDataLockCommandValidator : AbstractValidator<UpdateDataLockCommand>
    {
        public UpdateDataLockCommandValidator()
        {
            RuleFor(x => x.ApprenticeshipId).NotEmpty();
            RuleFor(x => x.ProviderId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.TriageStatus)
                .Must(m => m == TriageStatus.FixInIlr || m == TriageStatus.Change);
        }
    }
}