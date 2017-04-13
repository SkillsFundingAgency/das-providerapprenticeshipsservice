using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.ApprenticeshipUpdate
{
    public class ReviewApprenticeshipUpdateViewModelValidator : AbstractValidator<ReviewApprenticeshipUpdateViewModel>
    {
        public ReviewApprenticeshipUpdateViewModelValidator()
        {
            RuleFor(x => x.ApproveChanges).NotEmpty().WithMessage("Select an option");
        }
    }
}