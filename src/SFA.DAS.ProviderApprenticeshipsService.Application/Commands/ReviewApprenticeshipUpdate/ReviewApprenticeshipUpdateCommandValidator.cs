using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.ReviewApprenticeshipUpdate
{
    public sealed class ReviewApprenticeshipUpdateCommandValidator : AbstractValidator<ReviewApprenticeshipUpdateCommand>
    {
        public ReviewApprenticeshipUpdateCommandValidator()
        {
            RuleFor(x => x.ApprenticeshipId).NotEmpty();
            RuleFor(x => x.ProviderId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
