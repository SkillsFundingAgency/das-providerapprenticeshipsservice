using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class UploadApprenticeshipsViewModel
    {
        [Required(ErrorMessage = "Please choose a file to upload")]
        public HttpPostedFileBase Attachment { get; set; }

        public long ProviderId { get; set; }

        public string HashedCommitmentId { get; set; }

        public int ApprenticeshipCount { get; set; }

        public int ErrorCount { get; set; }

        public int WarningsCount { get; set; }

        public IEnumerable<UploadRowErrorViewModel> Errors { get; set; }

        public int RowCount { get; set; }

        public IEnumerable<UploadError> FileErrors { get; set; }

        public bool IsPaidByTransfer { get; set; }

        public string AccountLegalEntityPublicHashedId { get; set; } //AgreementId

        public bool BlackListed { get; set; }
    }
}