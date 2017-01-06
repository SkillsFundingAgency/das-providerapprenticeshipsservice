using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class UploadApprenticeshipsViewModel
    {
        [Required(ErrorMessage = "You must choose a file to upload")]
        public HttpPostedFileBase Attachment { get; set; }

        public long ProviderId { get; set; }

        public string HashedCommitmentId { get; set; }

        public int ApprenticeshipCount { get; set; }

        public int ErrorCount { get; set; }

        public int WarningsCount { get; set; }
    }
}