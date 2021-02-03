using System.Collections.Generic;
using System.Web;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public interface IBulkUploadValidator
    {
        IEnumerable<UploadError> ValidateRecords(IEnumerable<ApprenticeshipUploadModel> records, List<TrainingProgramme> trainingProgrammes);

        IEnumerable<UploadError> ValidateCohortReference( IEnumerable<ApprenticeshipUploadModel> records, string cohortReference);

        IEnumerable<UploadError> ValidateFileSize(HttpPostedFileBase attachment);
        IEnumerable<UploadError> ValidateUlnUniqueness(IEnumerable<ApprenticeshipUploadModel> uploadResultData);
    }
}