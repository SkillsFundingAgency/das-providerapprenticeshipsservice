using System;

using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    using System.Text.RegularExpressions;

    public sealed class ApprenticeshipViewModelValidator : ApprenticeshipBaseValidator // : AbstractValidator<ApprenticeshipViewModel>
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

    public sealed class ApprenticeshipBulkUploadValidator : ApprenticeshipBaseValidator
    {
        public ApprenticeshipBulkUploadValidator()
        {
            var now = DateTime.Now;
            var text = new ApprenticeshipValidationText();
            Func<string, int, bool> lengthLessThan = (str, lenth) => (str?.Length ?? 0) <= lenth;

            RuleFor(x => x.FirstName)
                .Must(m => !string.IsNullOrEmpty(m)).WithMessage(text.GivenNames01.Text).WithErrorCode(text.GivenNames01.ErrorCode)
                .Must(m => lengthLessThan(m, 100)).WithMessage(text.GivenNames02.Text).WithErrorCode(text.GivenNames02.ErrorCode);

            RuleFor(x => x.LastName)
                .Must(m => !string.IsNullOrEmpty(m)).WithMessage(text.FamilyName01.Text).WithErrorCode(text.FamilyName01.ErrorCode)
                .Must(m => lengthLessThan(m, 100)).WithMessage(text.FamilyName02.Text).WithErrorCode(text.FamilyName02.ErrorCode);

            RuleFor(x => x.Cost)
                .Matches("^$|^[1-9]{1}[0-9]*$").WithMessage(text.TrainingPrice01.Text).WithErrorCode(text.TrainingPrice01.ErrorCode)
                .Must(m => lengthLessThan(m, 6)).WithMessage(text.TrainingPrice02.Text).WithErrorCode(text.TrainingPrice02.ErrorCode);

            RuleFor(r => r.StartDate)
                .Must(base.ValidateStartDate).Unless(m => m.StartDate == null).WithMessage(text.LearnStartDate02.Text).WithErrorCode(text.LearnStartDate02.ErrorCode)
                .Must(m => CheckIfNotNull(m?.DateTime, m?.DateTime > now)).WithMessage("Learner start date must be in the future"); // ToDo: Delete?

            RuleFor(r => r.EndDate)
                .Must(ValidateStartDate).Unless(m => m.EndDate == null).WithMessage(text.LearnPlanEndDate02.Text).WithErrorCode(text.LearnPlanEndDate02.ErrorCode)
                .Must(BeGreaterThenStartDate).WithMessage(text.LearnPlanEndDate03.Text).WithErrorCode(text.LearnPlanEndDate03.ErrorCode)
                .Must(m => CheckIfNotNull(m?.DateTime, m?.DateTime > now)).WithMessage(text.LearnPlanEndDate06.Text).WithErrorCode(text.LearnPlanEndDate06.ErrorCode);
        }
    }

    public class ApprenticeshipBaseValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipBaseValidator()
        {
            var text = new ApprenticeshipValidationText();
            Func<string, int, bool> lengthLessThan = (str, lenth) => (str?.Length ?? 0) <= lenth;
            var yesterday = DateTime.Now.AddDays(-1);

            RuleFor(x => x.ULN)
                .Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage(text.Uln01.Text).WithErrorCode(text.Uln01.ErrorCode)
                .When(m => m.ULN == "9999999999").WithMessage(text.Uln02.Text).WithErrorCode(text.Uln02.ErrorCode);
            // ToDo: text.UlnPassChecksum
            // ToDo text.UlnAlreadyInUse

            RuleFor(r => r.DateOfBirth)
                .Must(ValidateDateOfBirth).Unless(m => m.DateOfBirth == null).WithMessage(text.DateOfBirth01.Text).WithErrorCode(text.DateOfBirth01.ErrorCode)
                .Must(m => CheckIfNotNull(m?.DateTime, m?.DateTime < yesterday)).WithMessage("Date of birth must be in the past"); // ToDo: Delete?
            // ToDo: Date of birth -> DateOfBirth_02 - DateOfBirth_06

            RuleFor(x => x.NINumber)
                .Must(m => lengthLessThan(m, 9)).WithMessage(text.NINumber02.Text).WithErrorCode(text.NINumber02.ErrorCode)
                .Matches(@"^[abceghj-prstw-z][abceghj-nprstw-z]\d{6}[abcd\s]$", RegexOptions.IgnoreCase).WithMessage(text.NINumber03.Text).WithErrorCode(text.NINumber03.ErrorCode);

            RuleFor(x => x.ProviderRef)
                .Must(m => lengthLessThan(m, 20)).WithMessage(text.ProviderRef01.Text).WithErrorCode(text.ProviderRef01.ErrorCode);
        }

        protected bool BeGreaterThenStartDate(ApprenticeshipViewModel viewModel, DateTimeViewModel date)
        {
            if (viewModel.StartDate?.DateTime == null || viewModel.EndDate?.DateTime == null) return true;

            return viewModel.StartDate.DateTime < viewModel.EndDate.DateTime;
        }

        protected readonly Func<DateTime?, bool, bool> CheckIfNotNull = (dt, b) => dt == null || b;

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

        protected bool ValidateStartDate(DateTimeViewModel date)
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