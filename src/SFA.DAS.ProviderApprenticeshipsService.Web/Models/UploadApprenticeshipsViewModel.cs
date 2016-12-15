using System.Web;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class UploadApprenticeshipsViewModel
    {
        public HttpPostedFileBase Attachment { get; set; }

        public long ProviderId { get; set; }

        public string HashedCommitmentId { get; set; }
    }
}