using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class DeleteCohortConfirmationViewModelValidator : AbstractValidator<DeleteCommitmentViewModel>
    {
        public DeleteCohortConfirmationViewModelValidator()
        {
            RuleFor(x => x.DeleteConfirmed).NotNull().WithMessage("Confirm deletion");
        }
    }
}