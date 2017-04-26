using System;

using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLock
{
    // ToDo: DCM-475 -> Use models from the API types when that is updated
    public class DataLockStatus
    {
        public long DataLockEventId { get; set; }
        public DateTime DataLockEventDatetime { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long ApprenticeshipId { get; set; }
        public string IlrTrainingCourseCode { get; set; }
        public TrainingType IlrTrainingType { get; set; }
        public DateTime? IlrActualStartDate { get; set; }
        public DateTime? IlrEffectiveFromDate { get; set; }
        public decimal? IlrTotalCost { get; set; }
        public Status Status { get; set; }
        public TriageStatus TriageStatus { get; set; }
        public DataLockErrorCode ErrorCode { get; set; }
    }

    public enum Status
    {
        Unknown = 0,
        Pass = 1,
        Fail = 2
    }

    public enum TriageStatus
    {
        Unknown = 0,
        Change = 1,
        Restart = 2,
        FixInIlr = 3
    }

    [Flags]
    public enum DataLockErrorCode
    {
        None = 0,
        Dlock01 = 1,
        Dlock02 = 2,
        Dlock03 = 4,
        Dlock04 = 8,
        Dlock05 = 16,
        Dlock06 = 32,
        Dlock07 = 64,
        Dlock08 = 128,
        Dlock09 = 256,
        Dlock10 = 512
    }
}