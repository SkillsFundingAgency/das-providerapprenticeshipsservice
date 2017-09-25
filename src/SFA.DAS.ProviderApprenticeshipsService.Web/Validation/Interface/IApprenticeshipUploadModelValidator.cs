using FluentValidation.Results;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public interface IApprenticeshipUploadModelValidator
    {
        ValidationResult Validate(ApprenticeshipUploadModel model);
    }
}