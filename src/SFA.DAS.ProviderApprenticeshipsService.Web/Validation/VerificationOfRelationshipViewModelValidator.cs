using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class VerificationOfRelationshipViewModelValidator : AbstractValidator<VerificationOfRelationshipViewModel>
    {
        public VerificationOfRelationshipViewModelValidator()
        {
            RuleFor(x => x.OrganisationIsSameOrConnected)
              .NotNull()
              .WithMessage("Please select an option");
        }
    }
}