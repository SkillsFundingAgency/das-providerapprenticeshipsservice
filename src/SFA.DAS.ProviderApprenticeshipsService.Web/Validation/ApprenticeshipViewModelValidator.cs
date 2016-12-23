using System;

using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class ApprenticeshipViewModelValidator : ApprenticeshipBaseValidator
    {
        public ApprenticeshipViewModelValidator()
        {
            var now = DateTime.Now;
            var text = new ApprenticeshipValidationText();
            Func<string, int, bool> lengthLessThan = (str, lenth) => (str?.Length ?? 0) <= lenth;

            RuleFor(x => x.FirstName)
                .Must(m => !string.IsNullOrEmpty(m)).WithMessage("First names must be entered")
                .Must(m => lengthLessThan(m, 100)).WithMessage("First names must be entered and must not be more than 100 characters in length");

            RuleFor(x => x.LastName)
                .Must(m => !string.IsNullOrEmpty(m)).WithMessage("Last name must be entered")
                .Must(m => lengthLessThan(m, 100)).WithMessage("The Last name must be entered and must not be more than 100 characters in length");

            RuleFor(x => x.Cost)
                .Matches("^$|^[1-9]{1}[0-9]*$").WithMessage("Cost must be entered")
                .Must(m => lengthLessThan(m, 6)).WithMessage("Cost must be entered and must not be more than 6 characters in length");

            RuleFor(r => r.StartDate)
                .Must(ValidateStartDate).Unless(m => m.StartDate == null).WithMessage("The Learning start end date is not valid");

            RuleFor(r => r.EndDate)
                .Must(ValidateStartDate).Unless(m => m.EndDate == null).WithMessage("The Learning planned end date is not valid")
                .Must(BeGreaterThenStartDate).WithMessage(text.LearnPlanEndDate03.Text)
                .Must(m => CheckIfNotNull(m?.DateTime, m?.DateTime > now)).WithMessage(text.LearnPlanEndDate06.Text);
        }
    }
}