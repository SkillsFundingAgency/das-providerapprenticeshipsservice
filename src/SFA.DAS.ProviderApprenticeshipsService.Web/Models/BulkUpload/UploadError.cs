using System;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    [Serializable]
    public class UploadError
    {
        public UploadError(string message)
        {
            Message = message;
            ErrorCode = string.Empty;
        }

        public UploadError(string message, string errorCode, int? index = null, ApprenticeshipUploadModel record = null)
        {
            Message = message;
            ErrorCode = errorCode;
            Row = index;
            IsGeneralError = index == null || record == null;
            Uln = record?.ApprenticeshipViewModel?.ULN;
            FirstName = record?.ApprenticeshipViewModel?.FirstName;
            LastName = record?.ApprenticeshipViewModel?.LastName;
            DateOfBirth = record?.ApprenticeshipViewModel?.DateOfBirth;
        }

        public bool IsGeneralError { get; set; }

        public DateTimeViewModel DateOfBirth { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Message { get; set; }

        public string ErrorCode { get; set; }

        public int?  Row { get; set; }

        public string Uln { get; set; }

        public override string ToString()
        {
            if(Row.HasValue)
                return $"Row:{Row} - {Message}";
            return $"{Message}";
        }
    }
}