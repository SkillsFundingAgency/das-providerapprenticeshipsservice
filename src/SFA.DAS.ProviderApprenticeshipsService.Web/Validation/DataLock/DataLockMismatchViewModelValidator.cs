using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.DataLock
{
    public class DataLockMismatchViewModelValidator : AbstractValidator<DataLockMismatchViewModel>
    {
        public DataLockMismatchViewModelValidator()
        {
            RuleFor(x => x.SubmitStatusViewModel)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                    .WithMessage("Select an option")
                .Must(m => m.HasValue && m.Value != SubmitStatusViewModel.UpdateDataInIlr)
                    .WithMessage("This option is currently not supported");
        }
    }
}