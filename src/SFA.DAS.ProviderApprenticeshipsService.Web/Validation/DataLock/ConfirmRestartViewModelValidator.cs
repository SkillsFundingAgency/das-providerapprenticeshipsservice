using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.DataLock
{
    public class ConfirmRestartViewModelValidator : AbstractValidator<ConfirmRestartViewModel>
    {
        public ConfirmRestartViewModelValidator()
        {
            RuleFor(x => x.SendRequestToEmployer).NotEmpty().WithMessage("Select an option");
        }
    }
}