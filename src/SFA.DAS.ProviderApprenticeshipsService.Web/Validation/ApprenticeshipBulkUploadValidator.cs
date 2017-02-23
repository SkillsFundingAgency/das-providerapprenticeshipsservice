using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public sealed class ApprenticeshipBulkUploadValidator : ApprenticeshipCoreValidator
    {
        public ApprenticeshipBulkUploadValidator(IApprenticeshipValidationErrorText validationText, ICurrentDateTime currentDateTime) : base(validationText, currentDateTime)
        {
        }

        protected override void ValidateTraining()
        {
            // Don't need to validate as this is done at the csv level.
            // base.ValidateTraining();
        }
    }
}