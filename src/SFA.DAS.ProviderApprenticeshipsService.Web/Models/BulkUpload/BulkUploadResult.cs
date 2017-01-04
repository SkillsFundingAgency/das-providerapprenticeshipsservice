using System.Collections.Generic;

using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    public class BulkUploadResult
    {
        public IEnumerable<UploadError> Errors { get; set; } = new List<UploadError>();

        public IEnumerable<ApprenticeshipUploadModel> Data { get; set; }
    }
}