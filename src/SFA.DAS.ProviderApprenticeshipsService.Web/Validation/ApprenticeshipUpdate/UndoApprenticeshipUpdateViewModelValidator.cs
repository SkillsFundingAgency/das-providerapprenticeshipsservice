using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.ApprenticeshipUpdate
{
    public class UndoApprenticeshipUpdateViewModelValidator : AbstractValidator<UndoApprenticeshipUpdateViewModel>
    {
        public UndoApprenticeshipUpdateViewModelValidator()
        {
            RuleFor(x => x.ConfirmUndo).NotEmpty().WithMessage("Select an option");
        }
    }
}