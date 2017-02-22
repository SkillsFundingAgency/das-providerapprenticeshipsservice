using System;
using System.Linq;
using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprenticeshipCoreValidator : AbstractValidator<ApprenticeshipViewModel>
    {
        protected static readonly Func<string, int, bool> LengthLessThanFunc = (str, length) => (str?.Length ?? length) < length;
        protected static readonly Func<DateTime?, bool, bool> CheckIfNotNull = (dt, b) => dt == null || b;
        protected static readonly Func<string, int, bool> HaveNumberOfDigitsFewerThan = (str, length) => { return (str?.Count(char.IsDigit) ?? 0) < length; };
        private readonly IApprenticeshipValidationErrorText _validationText;

        public ApprenticeshipCoreValidator(IApprenticeshipValidationErrorText validationText)
        {
            _validationText = validationText;

            ValidateFirstName();

            ValidateLastName();

            ValidateUln();

            ValidateDateOfBirth();
            
            ValidateStartDate();

            ValidateEndDate();

            ValidateCost();

            ValidateProviderReference();
        }

        private void ValidateFirstName()
        {
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(_validationText.GivenNames01.Text).WithErrorCode(_validationText.GivenNames01.ErrorCode)
                .NotEmpty().WithMessage(_validationText.GivenNames01.Text).WithErrorCode(_validationText.GivenNames01.ErrorCode)
                .Must(m => LengthLessThanFunc(m, 101)).WithMessage(_validationText.GivenNames02.Text).WithErrorCode(_validationText.GivenNames02.ErrorCode);
        }

        private void ValidateLastName()
        {
            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(_validationText.FamilyName01.Text).WithErrorCode(_validationText.FamilyName01.ErrorCode)
                .NotEmpty().WithMessage(_validationText.FamilyName01.Text).WithErrorCode(_validationText.FamilyName01.ErrorCode)
                .Must(m => LengthLessThanFunc(m, 101)).WithMessage(_validationText.FamilyName02.Text).WithErrorCode(_validationText.FamilyName02.ErrorCode); ;
        }

        protected virtual void ValidateUln()
        {
            RuleFor(x => x.ULN)
                .NotNull().WithMessage(_validationText.Uln01.Text).WithErrorCode(_validationText.Uln01.ErrorCode)
                .Matches("^[1-9]{1}[0-9]{9}$").WithMessage(_validationText.Uln01.Text).WithErrorCode(_validationText.Uln01.ErrorCode)
                .Must(m => m != "9999999999").WithMessage(_validationText.Uln02.Text).WithErrorCode(_validationText.Uln02.ErrorCode);
        }

        protected virtual void ValidateDateOfBirth()
        {
            RuleFor(r => r.DateOfBirth)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(_validationText.DateOfBirth01.Text).WithErrorCode(_validationText.DateOfBirth01.ErrorCode)
                .Must(ValidateDateOfBirth).WithMessage(_validationText.DateOfBirth01.Text).WithErrorCode(_validationText.DateOfBirth01.ErrorCode);
        }

        protected virtual void ValidateStartDate()
        {
            RuleFor(x => x.StartDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(_validationText.LearnStartDate01.Text).WithErrorCode(_validationText.LearnStartDate01.ErrorCode)
                .Must(ValidateDateWithoutDay).WithMessage(_validationText.LearnStartDate01.Text).WithErrorCode(_validationText.LearnStartDate01.ErrorCode)
                .Must(NotBeBeforeMay2017).WithMessage(_validationText.LearnStartDate03.Text).WithErrorCode(_validationText.LearnStartDate03.ErrorCode);
        }

        protected virtual void ValidateEndDate()
        {
            RuleFor(x => x.EndDate)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(_validationText.LearnPlanEndDate02.Text).WithErrorCode(_validationText.LearnPlanEndDate02.ErrorCode)
                .Must(ValidateDateWithoutDay).WithMessage(_validationText.LearnPlanEndDate02.Text).WithErrorCode(_validationText.LearnPlanEndDate02.ErrorCode)
                .Must(BeGreaterThenStartDate).WithMessage(_validationText.LearnPlanEndDate03.Text).WithErrorCode(_validationText.LearnPlanEndDate03.ErrorCode)
                .Must(m => m.DateTime > DateTime.UtcNow).WithMessage(_validationText.LearnPlanEndDate06.Text).WithErrorCode(_validationText.LearnPlanEndDate06.ErrorCode);
        }

        protected virtual void ValidateCost()
        {
            RuleFor(x => x.Cost)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(_validationText.TrainingPrice01.Text).WithErrorCode(_validationText.TrainingPrice01.ErrorCode)
                .Matches("^([1-9]{1}([0-9]{1,2})?)+(,[0-9]{3})*$|^[1-9]{1}[0-9]*$").WithMessage(_validationText.TrainingPrice01.Text).WithErrorCode(_validationText.TrainingPrice01.ErrorCode)
                .Must(m => HaveNumberOfDigitsFewerThan(m, 7)).WithMessage(_validationText.TrainingPrice02.Text).WithErrorCode(_validationText.TrainingPrice02.ErrorCode)
                .Must(m => decimal.Parse(m) <= 100000).WithMessage(_validationText.TrainingPrice03.Text).WithErrorCode(_validationText.TrainingPrice03.ErrorCode);
        }

        private void ValidateProviderReference()
        {
            RuleFor(x => x.ProviderRef)
                .Must(m => LengthLessThanFunc(m, 20))
                    .When(x => !string.IsNullOrEmpty(x.ProviderRef)).WithMessage(_validationText.ProviderRef01.Text).WithErrorCode(_validationText.ProviderRef01.ErrorCode);
        }

        private bool BeGreaterThenStartDate(ApprenticeshipViewModel viewModel, DateTimeViewModel date)
        {
            if (viewModel.StartDate?.DateTime == null || viewModel.EndDate?.DateTime == null) return true;

            return viewModel.StartDate.DateTime < viewModel.EndDate.DateTime;
        }

        private bool ValidateDateWithoutDay(DateTimeViewModel date)
        {
            return date.DateTime != null;
        }

        private bool NotBeBeforeMay2017(DateTimeViewModel date)
        {
            return date.DateTime >= new DateTime(2017, 5, 1);
        }

        private bool ValidateDateOfBirth(DateTimeViewModel date)
        {
            // Check the day has value as the view model supports just month and year entry
            if (date.DateTime == null || !date.Day.HasValue) return false;

            return true;
        }
    }
}