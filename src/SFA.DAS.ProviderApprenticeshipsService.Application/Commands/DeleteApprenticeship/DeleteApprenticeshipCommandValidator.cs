using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteApprenticeship
{
    public sealed class DeleteApprenticeshipCommandValidator : AbstractValidator<DeleteApprenticeshipCommand>
    {
        public DeleteApprenticeshipCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.ApprenticeshipId).GreaterThan(0);
        }
    }
}
