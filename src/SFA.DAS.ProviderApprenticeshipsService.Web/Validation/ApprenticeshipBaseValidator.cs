using System;
using System.Linq;
using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public abstract class ApprenticeshipBaseValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        protected static readonly Func<string, int, bool> LengthLessThanFunc = (str, length) => (str?.Length ?? length) < length;
        protected static readonly Func<DateTime?, bool, bool> CheckIfNotNull = (dt, b) => dt == null || b;
        protected static readonly Func<string, int, bool> HaveNumberOfDigitsFewerThan = (str, length) => { return (str?.Count(char.IsDigit) ?? 0) < length; };
        private readonly IApprenticeshipValidationErrorText _validationText;

        public ApprenticeshipBaseValidator(IApprenticeshipValidationErrorText validationText)
        {
            _validationText = validationText;

            RuleFor(x => x.ULN)
                .Matches("^$|^[1-9]{1}[0-9]{9}$").WithMessage(_validationText.Uln01.Text).WithErrorCode(_validationText.Uln01.ErrorCode)
                .Must(m => m != "9999999999").WithMessage(_validationText.Uln02.Text).WithErrorCode(_validationText.Uln02.ErrorCode);

            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(_validationText.GivenNames01.Text).WithErrorCode(_validationText.GivenNames01.ErrorCode)
                .NotEmpty().WithMessage(_validationText.GivenNames01.Text).WithErrorCode(_validationText.GivenNames01.ErrorCode)
                .Must(m => LengthLessThanFunc(m, 101)).WithMessage(_validationText.GivenNames02.Text).WithErrorCode(_validationText.GivenNames02.ErrorCode);

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(_validationText.FamilyName01.Text).WithErrorCode(_validationText.FamilyName01.ErrorCode)
                .NotEmpty().WithMessage(_validationText.FamilyName01.Text).WithErrorCode(_validationText.FamilyName01.ErrorCode)
                .Must(m => LengthLessThanFunc(m, 101)).WithMessage(_validationText.FamilyName02.Text).WithErrorCode(_validationText.FamilyName02.ErrorCode); ;

            ValidateDateOfBirth();

            ValidateStartDate();

            ValidateEndDate();

            ValidateCost();

            ValidateProviderReference();
        }

        protected virtual void ValidateDateOfBirth()
        {
            RuleFor(r => r.DateOfBirth)
                .Must(ValidateDateOfBirth).Unless(m => m.DateOfBirth == null).WithMessage(_validationText.DateOfBirth01.Text).WithErrorCode(_validationText.DateOfBirth01.ErrorCode);
        }

        protected virtual void ValidateProviderReference()
        {
            RuleFor(x => x.ProviderRef)
                .Must(m => LengthLessThanFunc(m, 20))
                .When(x => !string.IsNullOrEmpty(x.ProviderRef)).WithMessage(_validationText.ProviderRef01.Text).WithErrorCode(_validationText.ProviderRef01.ErrorCode);
        }

        protected virtual void ValidateCost()
        {
            RuleFor(x => x.Cost)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Matches("^$|^([1-9]{1}([0-9]{1,2})?)+(,[0-9]{3})*$|^[1-9]{1}[0-9]*$").When(m => HaveNumberOfDigitsFewerThan(m.Cost, 7)).WithMessage(_validationText.TrainingPrice01.Text).WithErrorCode(_validationText.TrainingPrice01.ErrorCode)
                .Must(m => HaveNumberOfDigitsFewerThan(m, 7)).WithMessage(_validationText.TrainingPrice02.Text).WithErrorCode(_validationText.TrainingPrice02.ErrorCode)
                .Must(m => (m == null ? default(decimal?) : decimal.Parse(m)) <= 100000).When(m => m.Cost?.Length > 0).WithMessage(_validationText.TrainingPrice03.Text).WithErrorCode(_validationText.TrainingPrice03.ErrorCode);
        }

        protected virtual void ValidateStartDate()
        {
            When(x => HasYearOrMonthValueSet(x.StartDate), () => 
            {
                RuleFor(x => x.StartDate)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .Must(ValidateDateWithoutDay).WithMessage(_validationText.LearnStartDate01.Text).WithErrorCode(_validationText.LearnStartDate01.ErrorCode)
                    .Must(NotBeBeforeMay2017).WithMessage(_validationText.LearnStartDate03.Text).WithErrorCode(_validationText.LearnStartDate03.ErrorCode);
            });
        }

        protected virtual void ValidateEndDate()
        {
            When(x => HasYearOrMonthValueSet(x.EndDate), () =>
            {
                RuleFor(x => x.EndDate)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .Must(ValidateDateWithoutDay).WithMessage(_validationText.LearnPlanEndDate02.Text).WithErrorCode(_validationText.LearnPlanEndDate02.ErrorCode)
                    .Must(BeGreaterThenStartDate).WithMessage(_validationText.LearnPlanEndDate03.Text).WithErrorCode(_validationText.LearnPlanEndDate03.ErrorCode)
                .Must(m => CheckIfNotNull(m?.DateTime, m?.DateTime > DateTime.UtcNow)).WithMessage(_validationText.LearnPlanEndDate06.Text).WithErrorCode(_validationText.LearnPlanEndDate06.ErrorCode);
            });
        }

        protected bool BeGreaterThenStartDate(ApprenticeshipViewModel viewModel, DateTimeViewModel date)
        {
            if (viewModel.StartDate?.DateTime == null || viewModel.EndDate?.DateTime == null) return true;

            return viewModel.StartDate.DateTime < viewModel.EndDate.DateTime;
        }

        protected bool ValidateDateWithoutDay(DateTimeViewModel date)
        {
            if (date.DateTime == null)
            {
                if (!date.Month.HasValue && !date.Year.HasValue) return true;

                return false;
            }

            return true;
        }

        private bool HasYearOrMonthValueSet(DateTimeViewModel date)
        {
            if (date == null) return false;

            if (date.Day.HasValue || date.Month.HasValue || date.Year.HasValue) return true;

            return false;
        }

        private bool NotBeBeforeMay2017(DateTimeViewModel date)
        {
            return date.DateTime >= new DateTime(2017, 5, 1);
        }

        private bool ValidateDateOfBirth(DateTimeViewModel date)
        {
            if (date.DateTime == null)
            {
                if (!date.Day.HasValue && !date.Month.HasValue && !date.Year.HasValue) return true;
                return false;
            }

            // Partially populated date should fail
            if (!date.Day.HasValue || !date.Month.HasValue || !date.Year.HasValue) return false;

            return true;
        }
    }
}