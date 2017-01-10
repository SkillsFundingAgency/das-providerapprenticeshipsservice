using System;
using System.Linq;
using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class ApprenticeshipViewModelValidator : ApprenticeshipBaseValidator
    {
        public ApprenticeshipViewModelValidator()
        {
            var now = DateTime.Now;
            var yesterday = DateTime.Now.AddDays(-1);
            var text = new ApprenticeshipValidationText();
            Func<string, int, bool> lengthLessThan = (str, length) => (str?.Length ?? 0) <= length;
            Func<string, int, bool> numberOfDigitsLessThan = (str, length) => { return (str?.Count(char.IsDigit) ?? 0) < length; };
            Func<DateTime?, bool, bool> _checkIfNotNull = (dt, b) => dt == null || b;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First names must be entered")
                .Must(m => lengthLessThan(m, 100)).WithMessage("You must enter a first name that's no longer than 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name must be entered")
                .Must(m => lengthLessThan(m, 100)).WithMessage("You must enter a last name that's no longer than 100 characters");

            RuleFor(x => x.Cost)
                .Matches("^$|^([1-9]{1}([0-9]{1,2})?)+(,[0-9]{3})*$").When(m => numberOfDigitsLessThan(m.Cost, 7)).WithMessage("Enter the total agreed training cost")
                .Must(m => numberOfDigitsLessThan(m, 7)).WithMessage("The cost must be 6 numbers or fewer, for example 25000");

            RuleFor(r => r.StartDate)
                .Must(ValidateDateWithoutDay).Unless(m => m.StartDate == null).WithMessage("The Learning start end date is not valid");

            RuleFor(r => r.EndDate)
                            .Must(ValidateDateWithoutDay).Unless(m => m.EndDate == null).WithMessage("The Learning planned end date is not valid")
                            .Must(BeGreaterThenStartDate).WithMessage(text.LearnPlanEndDate03.Text)
                            .Must(m => CheckIfNotNull(m?.DateTime, m?.DateTime > now)).WithMessage(text.LearnPlanEndDate06.Text);

            RuleFor(r => r.DateOfBirth)
                .Must(m => _checkIfNotNull(m?.DateTime, m?.DateTime < yesterday)).WithMessage("The date of birth must be in the past");
        }
    }
}