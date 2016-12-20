using System.Collections.Generic;
using System.Web;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class UploadApprenticeshipsViewModel
    {
        public HttpPostedFileBase Attachment { get; set; }

        public long ProviderId { get; set; }

        public string HashedCommitmentId { get; set; }

        public IEnumerable<UploadError> Errors { get; set; }
    }
}