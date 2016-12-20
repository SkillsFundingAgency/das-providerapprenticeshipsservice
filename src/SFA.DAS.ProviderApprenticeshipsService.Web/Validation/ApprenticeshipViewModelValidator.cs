using System;

using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    using System.Text.RegularExpressions;

    public sealed class ApprenticeshipViewModelValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipViewModelValidator()
        {
            Func<string, int, bool> lengthLessThan = (str, lenth) => (str?.Length ?? 0) <= lenth;
            RuleFor(x => x.ULN).Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage("Enter a valid unique learner number");

            var now = DateTime.Now;
            var yesterday= DateTime.Now.AddDays(-1);

            RuleFor(x => x.FirstName)
                .Must(m => !string.IsNullOrEmpty(m)).WithMessage("Enter a first name").WithErrorCode("GivenNames_01")
                .Must(m => lengthLessThan(m, 100)).WithMessage("First name cannot contain more then 100 chatacters").WithErrorCode("GivenNames_02");

            RuleFor(x => x.LastName)
                .Must(m => !string.IsNullOrEmpty(m)).WithMessage("Enter a last name").WithErrorCode("FamilyName_01")
                .Must(m => lengthLessThan(m, 100)).WithMessage("Last name cannot contain more then 100 chatacters").WithErrorCode("FamilyName_02");

            RuleFor(x => x.NINumber)
                .Must(m => lengthLessThan(m, 9)).WithMessage("National insurance number needs to be 10 characters long").WithErrorCode("NINumber_02")
                .Matches(@"^[abceghj-prstw-z][abceghj-nprstw-z]\d{6}[abcd\s]$", RegexOptions.IgnoreCase).WithMessage("Enter a valid national insurance number").WithErrorCode("NINumber_03");

            RuleFor(r => r.StartDate)
                .Must(ValidateStartDate).Unless(m => m.StartDate == null).WithMessage("Start date is not a valid date")
                .Must(m => _checkIfNotNull(m?.DateTime, m?.DateTime > now)).WithMessage("Learner start date must be in the future");

            RuleFor(r => r.EndDate)
                .Must(ValidateStartDate).Unless(m => m.EndDate == null).WithMessage("Planed end date is not a valid date")
                .Must(BeGreaterThenStartDate).WithMessage("Learner planed end date must be greater than start date")
                .Must(m => _checkIfNotNull(m?.DateTime, m?.DateTime > now)).WithMessage("Learner planed end date must be in the future");

            RuleFor(r => r.DateOfBirth)
                .Must(ValidateDateOfBirth).Unless(m => m.DateOfBirth == null).WithMessage("Date of birth is not valid")
                .Must(m => _checkIfNotNull(m?.DateTime, m?.DateTime < yesterday)).WithMessage("Date of birth must be in the past");

            RuleFor(x => x.Cost).Matches("^$|^[1-9]{1}[0-9]*$").WithMessage("Enter the total agreed training cost");

            RuleFor(x => x.ProviderRef)
                .Must(m => lengthLessThan(m, 20)).WithMessage("Provider reference must not contain more than 20 characters").WithErrorCode("ProvRef_01");
        }

        private bool BeGreaterThenStartDate(ApprenticeshipViewModel viewModel, DateTimeViewModel date)
        {
            if (viewModel.StartDate?.DateTime == null || viewModel.EndDate?.DateTime == null) return true;

            return viewModel.StartDate.DateTime < viewModel.EndDate.DateTime;
        }

        private readonly Func<DateTime?, bool, bool> _checkIfNotNull = (dt, b) => dt == null || b ;

        private bool ValidateDateOfBirth(DateTimeViewModel date)
        {
            if (date.DateTime == null)
            {
                if (!date.Day.HasValue && !date.Month.HasValue && !date.Year.HasValue) return true;
                return false;
            }

            if (!date.Day.HasValue || !date.Month.HasValue || !date.Year.HasValue) return false;

            return true;
        }

        private bool ValidateStartDate(DateTimeViewModel date)
        {
            if (date.DateTime == null)
            {
                if (!date.Day.HasValue && !date.Month.HasValue && !date.Year.HasValue) return true;
                return false;
            }

            if (!date.Month.HasValue || !date.Year.HasValue) return false;

            return true;
        }
    }
}