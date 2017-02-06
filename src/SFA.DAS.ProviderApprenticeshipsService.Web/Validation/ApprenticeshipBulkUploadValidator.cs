using System;

using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class ApprenticeshipBulkUploadValidator : ApprenticeshipBaseValidator
    {
        private readonly ApprenticeshipValidationText _validationText;

        public ApprenticeshipBulkUploadValidator(ApprenticeshipValidationText validationText) : base(validationText)
        {
            _validationText = validationText;
            var now = DateTime.Now;

            Func<string, int, bool> lengthLessThan = (str, lenth) => (str?.Length ?? 0) <= lenth;

            RuleFor(x => x.FirstName)
                .Must(m => lengthLessThan(m, 100)).WithMessage(_validationText.GivenNames02.Text).WithErrorCode(_validationText.GivenNames02.ErrorCode)
                .NotEmpty().Unless(m => m.FirstName == null).WithMessage(_validationText.GivenNames02.Text).WithErrorCode(_validationText.GivenNames02.ErrorCode)
                .NotNull().WithMessage(_validationText.GivenNames01.Text).WithErrorCode(_validationText.GivenNames01.ErrorCode);

            RuleFor(x => x.LastName)
                .Must(m => lengthLessThan(m, 100)).WithMessage(_validationText.FamilyName02.Text).WithErrorCode(_validationText.FamilyName02.ErrorCode)
                .NotEmpty().Unless(m => m.LastName == null).WithMessage(_validationText.FamilyName02.Text).WithErrorCode(_validationText.FamilyName02.ErrorCode)
                .NotNull().WithMessage(_validationText.FamilyName01.Text).WithErrorCode(_validationText.FamilyName01.ErrorCode);

            RuleFor(x => x.Cost)
                .Matches("^$|^[1-9]{1}[0-9]*$").WithMessage(_validationText.TrainingPrice01.Text).WithErrorCode(_validationText.TrainingPrice01.ErrorCode)
                .Must(m => lengthLessThan(m, 6)).WithMessage(_validationText.TrainingPrice02.Text).WithErrorCode(_validationText.TrainingPrice02.ErrorCode);

            RuleFor(r => r.StartDate)
                .Must(base.ValidateDateWithoutDay).Unless(m => m.StartDate == null).WithMessage(_validationText.LearnStartDate02.Text).WithErrorCode(_validationText.LearnStartDate02.ErrorCode)
                .Must(m => CheckIfNotNull(m?.DateTime, m?.DateTime > now)).WithMessage("Learner start date must be in the future"); // ToDo: Delete?

            RuleFor(r => r.EndDate)
                .Must(ValidateDateWithoutDay).Unless(m => m.EndDate == null).WithMessage(_validationText.LearnPlanEndDate02.Text).WithErrorCode(_validationText.LearnPlanEndDate02.ErrorCode)
                .Must(BeGreaterThenStartDate).WithMessage(_validationText.LearnPlanEndDate03.Text).WithErrorCode(_validationText.LearnPlanEndDate03.ErrorCode)
                .Must(m => CheckIfNotNull(m?.DateTime, m?.DateTime > now)).WithMessage(_validationText.LearnPlanEndDate06.Text).WithErrorCode(_validationText.LearnPlanEndDate06.ErrorCode);
        }
    }
}