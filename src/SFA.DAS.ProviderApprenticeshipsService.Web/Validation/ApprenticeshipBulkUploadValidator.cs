using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class ApprenticeshipBulkUploadValidator : ApprenticeshipBaseValidator
    {
        public ApprenticeshipBulkUploadValidator(IApprenticeshipValidationErrorText validationText) : base(validationText)
        {
            RuleFor(x => x.DateOfBirth).Must(x => x.DateTime != null).WithMessage(validationText.DateOfBirth01.Text).WithErrorCode(validationText.DateOfBirth01.ErrorCode);
        }
    }
}