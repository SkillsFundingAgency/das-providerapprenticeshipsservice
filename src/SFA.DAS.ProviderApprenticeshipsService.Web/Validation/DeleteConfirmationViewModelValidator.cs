using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class DeleteConfirmationViewModelValidator : AbstractValidator<DeleteConfirmationViewModel>
    {
        public DeleteConfirmationViewModelValidator()
        {
            RuleFor(x => x.DeleteConfirmed).NotNull().WithMessage("Confirm deletion");
        }
    }
}