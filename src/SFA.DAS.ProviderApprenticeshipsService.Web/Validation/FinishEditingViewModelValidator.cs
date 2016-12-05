using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class FinishEditingViewModelValidator : AbstractValidator<FinishEditingViewModel>
    {
        public FinishEditingViewModelValidator()
        {
            RuleFor(x => x.SaveStatus).IsInEnum().WithMessage("Select an option");
        }
    }
}