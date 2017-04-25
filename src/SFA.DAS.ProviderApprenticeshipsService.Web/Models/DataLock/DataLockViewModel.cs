using System;

using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock
{
    public class DataLockViewModel
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

        public TriageStatus TriageStatus { get; set; }
    }

    public enum TriageStatus
    {
        Unknown = 0,
        ChangeApprenticeship = 1,
        RestartApprenticeship = 2,
        FixInIlr = 3
    }
}