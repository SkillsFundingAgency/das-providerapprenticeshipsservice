using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class ApprenticeshipBulkUploadValidator : ApprenticeshipBaseValidator
    {
        public ApprenticeshipBulkUploadValidator(IApprenticeshipValidationErrorText validationText) : base(validationText)
        {
        }
    }
}