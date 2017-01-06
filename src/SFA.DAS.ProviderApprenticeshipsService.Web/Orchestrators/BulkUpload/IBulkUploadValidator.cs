using System.Collections.Generic;
using System.Web;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public interface IBulkUploadValidator
    {
        IEnumerable<UploadError> ValidateFields(IEnumerable<ApprenticeshipUploadModel> records, List<ITrainingProgramme> trainingProgrammes, string cohortReference);
        IEnumerable<UploadError> ValidateFileAttributes(HttpPostedFileBase attachment);
    }
}