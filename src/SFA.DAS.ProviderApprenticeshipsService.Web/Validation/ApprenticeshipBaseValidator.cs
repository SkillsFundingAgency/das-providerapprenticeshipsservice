using System;
using System.Text.RegularExpressions;

using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprenticeshipBaseValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        public ApprenticeshipBaseValidator()
        {
            var text = new ApprenticeshipValidationText();
            Func<string, int, bool> lengthLessThan = (str, lenth) => (str?.Length ?? 0) <= lenth;

            RuleFor(x => x.ULN)
                .Matches("^$|^[1-9]{1}[0-9]{9}$").When(m => lengthLessThan(m.ULN, 11)).WithMessage(text.Uln01.Text).WithErrorCode(text.Uln01.ErrorCode)
                .Must(m => lengthLessThan(m, 11)).WithMessage(text.Uln01.Text).WithErrorCode(text.Uln01.ErrorCode)
                .Must(m => m != "9999999999").WithMessage(text.Uln02.Text).WithErrorCode(text.Uln02.ErrorCode);
            // ToDo: text.UlnPassChecksum
            // ToDo text.UlnAlreadyInUse

            RuleFor(r => r.DateOfBirth)
                .Must(ValidateDateOfBirth).Unless(m => m.DateOfBirth == null).WithMessage(text.DateOfBirth01.Text).WithErrorCode(text.DateOfBirth01.ErrorCode);
            // ToDo: Date of birth -> DateOfBirth_02 - DateOfBirth_06

            RuleFor(x => x.NINumber)
                .Matches(@"^[abceghj-prstw-z][abceghj-nprstw-z]\d{6}[abcd\s]$", RegexOptions.IgnoreCase)
                    .Unless(m => m.NINumber == null || m.NINumber.Length > 9).WithMessage(text.NINumber03.Text).WithErrorCode(text.NINumber03.ErrorCode)
                .Must(m => lengthLessThan(m, 9)).WithMessage(text.NINumber02.Text).WithErrorCode(text.NINumber02.ErrorCode);

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

        protected bool ValidateDateWithoutDay(DateTimeViewModel date)
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