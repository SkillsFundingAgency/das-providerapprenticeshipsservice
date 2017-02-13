using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteCommitment
{
    public sealed class DeleteCommitmentCommandValidator : AbstractValidator<DeleteCommitmentCommand>
    {
        public DeleteCommitmentCommandValidator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.CommitmentId).GreaterThan(0);
        }
    }
}