using FluentValidation;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateDataLock
{
    public sealed class UpdateDataLockCommandValidator : AbstractValidator<UpdateDataLockCommand>
    {
        public UpdateDataLockCommandValidator()
        {
            RuleFor(x => x.ApprenticeshipId).NotEmpty();
            RuleFor(x => x.DataLockEventId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.TriageStatus)
                .Must(m => m != TriageStatus.Unknown );
        }
    }
}