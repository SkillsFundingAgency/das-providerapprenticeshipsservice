using System;

using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
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
}