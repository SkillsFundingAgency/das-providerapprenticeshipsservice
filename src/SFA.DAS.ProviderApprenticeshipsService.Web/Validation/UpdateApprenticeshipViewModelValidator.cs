using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class UpdateApprenticeshipViewModelValidator : AbstractValidator<UpdateApprenticeshipViewModel>
    {
        public UpdateApprenticeshipViewModelValidator()
        {
            RuleFor(x => x.ChangesConfirmed).NotEmpty().WithMessage("Select an option");
        }
    }
}