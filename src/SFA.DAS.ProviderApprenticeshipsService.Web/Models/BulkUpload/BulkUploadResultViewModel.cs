using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{

    public sealed class BulkUploadResultViewModel
    {
        public long BulkUploadId { get; set; }
        public bool HasFileLevelErrors { get; set; }
        public IEnumerable<UploadError> FileLevelErrors { get; set; }

        public bool HasRowLevelErrors { get; set; }
        public IEnumerable<UploadError> RowLevelErrors { get; set; }
    }
}