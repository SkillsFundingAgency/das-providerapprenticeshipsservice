using FluentValidation;

using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment
{
    public sealed class SubmitCommitmentCommandValidator : AbstractValidator<SubmitCommitmentCommand>
    {
        public SubmitCommitmentCommandValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.HashedCommitmentId).NotEmpty();
            RuleFor(x => x.CommitmentId).GreaterThan(0);
            RuleFor(x => x.LastAction).NotEqual(LastAction.None);
            RuleFor(x => x.UserDisplayName).NotEmpty();
            RuleFor(x => x.UserEmailAddress).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}