using System;
using System.Linq;
using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class ApprenticeshipViewModelValidator : ApprenticeshipBaseValidator
    {
        public ApprenticeshipViewModelValidator(WebApprenticeshipValidationText validationText) : base(validationText)
        {
            var now = DateTime.Now;
            var yesterday = DateTime.Now.AddDays(-1);
            var text = new WebApprenticeshipValidationText();
            Func<string, int, bool> lengthLessThan = (str, length) => (str?.Length ?? 0) <= length;
            Func<string, int, bool> haveNumberOfDigitsFewerThan = (str, length) => { return (str?.Count(char.IsDigit) ?? 0) < length; };
            Func<DateTime?, bool, bool> _checkIfNotNull = (dt, b) => dt == null || b;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(validationText.GivenNames01.Text).WithErrorCode(validationText.GivenNames01.ErrorCode)
                .Must(m => lengthLessThan(m, 100)).WithMessage(validationText.GivenNames02.Text).WithErrorCode(validationText.GivenNames02.ErrorCode);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(validationText.FamilyName01.Text).WithErrorCode(validationText.FamilyName01.ErrorCode)
                .Must(m => lengthLessThan(m, 100)).WithMessage(validationText.FamilyName02.Text).WithErrorCode(validationText.FamilyName02.ErrorCode);

            RuleFor(x => x.Cost)
                .Matches("^$|^([1-9]{1}([0-9]{1,2})?)+(,[0-9]{3})*$|^[1-9]{1}[0-9]*$").When(m => haveNumberOfDigitsFewerThan(m.Cost, 7)).WithMessage(validationText.TrainingPrice01.Text).WithErrorCode(validationText.TrainingPrice01.ErrorCode)
                .Must(m => haveNumberOfDigitsFewerThan(m, 7)).WithMessage(validationText.TrainingPrice02.Text).WithErrorCode(validationText.TrainingPrice02.ErrorCode);

            RuleFor(r => r.StartDate)
                .Must(ValidateDateWithoutDay).Unless(m => m.StartDate == null).WithMessage(validationText.LearnStartDate01.Text).WithErrorCode(validationText.LearnStartDate01.ErrorCode);

            RuleFor(r => r.EndDate)
                            .Must(ValidateDateWithoutDay).Unless(m => m.EndDate == null).WithMessage(validationText.LearnPlanEndDate01.Text).WithErrorCode(validationText.LearnPlanEndDate01.ErrorCode)
                            .Must(BeGreaterThenStartDate).WithMessage(text.LearnPlanEndDate03.Text)
                            .Must(m => CheckIfNotNull(m?.DateTime, m?.DateTime > now)).WithMessage(text.LearnPlanEndDate06.Text);

            RuleFor(r => r.DateOfBirth)
                .Must(m => _checkIfNotNull(m?.DateTime, m?.DateTime < yesterday)).WithMessage(validationText.DateOfBirth03.Text).WithErrorCode(validationText.DateOfBirth03.ErrorCode);
        }
    }
}