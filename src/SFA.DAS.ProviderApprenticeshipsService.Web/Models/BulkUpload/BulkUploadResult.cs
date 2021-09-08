using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    public class BulkUploadResult
    {
        public IEnumerable<UploadError> Errors { get; set; } = new List<UploadError>();

        public IEnumerable<ApprenticeshipUploadModel> Data { get; set; }

        public bool HasErrors => (Errors != null && Errors.Any());

        public long BulkUploadId { get; set; }
    }
}