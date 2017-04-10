using FluentValidation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate
{
    public class CreateApprenticeshipUpdateCommandValidator : AbstractValidator<CreateApprenticeshipUpdateCommand>
    {
        public CreateApprenticeshipUpdateCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProviderId).NotEmpty();
            RuleFor(x => x.ApprenticeshipUpdate).NotEmpty();
            RuleFor(x => x.ApprenticeshipUpdate.ApprenticeshipId).NotEmpty();
        }
    }
}
