using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.DataLock
{
    public class DataLockMismatchViewModelValidator : AbstractValidator<DataLockMismatchViewModel>
    {
        public DataLockMismatchViewModelValidator()
        {
            RuleFor(x => x.SubmitStatusViewModel)
                .Must(m => m != null && m.Value != SubmitStatusViewModel.UpdateDataInIlr).WithMessage("Select an option");
        }
    }
}