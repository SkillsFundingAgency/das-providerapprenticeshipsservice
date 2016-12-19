using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    public class BulkUploadResult
    {
        public IEnumerable<UploadError> Errors { get; set; }
    }

    public class UploadError
    {
        private string v;

        public UploadError(string message)
        {
            Message = message;
            ErrorCode = string.Empty;
        }

        public UploadError(string message, string errorCode, int? index = null)
        {
            Message = message;
            ErrorCode = errorCode;
            Row = index;
        }

        public string Message { get; set; }

        public string ErrorCode { get; set; }

        public int?  Row { get; set; }

        public override string ToString()
        {
            if(Row.HasValue)
                return $"Row:{Row} - {Message}";
            return $"{Message}";
        }
    }
}