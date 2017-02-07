using System.Collections.Generic;
using System.Web;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public interface IBulkUploadValidator
    {
        IEnumerable<UploadError> ValidateRecords(IEnumerable<ApprenticeshipUploadModel> records, List<ITrainingProgramme> trainingProgrammes);

        IEnumerable<UploadError> ValidateCohortReference( IEnumerable<ApprenticeshipUploadModel> records, string cohortReference);

        IEnumerable<UploadError> ValidateFileSize(HttpPostedFileBase attachment);
    }
}