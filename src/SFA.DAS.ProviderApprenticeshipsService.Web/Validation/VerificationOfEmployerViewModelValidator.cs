using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class VerificationOfEmployerViewModelValidator: AbstractValidator<VerificationOfEmployerViewModel>
    {
        public VerificationOfEmployerViewModelValidator()
        {
            RuleFor(x => x.ConfirmProvisionOfTrainingForOrganisation)
                .NotNull()
                .WithMessage("Please select an option");
        }
    }
}