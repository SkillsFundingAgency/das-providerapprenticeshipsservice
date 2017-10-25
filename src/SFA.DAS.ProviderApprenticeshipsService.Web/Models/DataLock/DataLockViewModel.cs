using System;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;
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

        public string IlrTrainingCourseName { get; set; }

        public TrainingType IlrTrainingType { get; set; }

        public DateTime? IlrActualStartDate { get; set; }

        public DateTime? IlrEffectiveFromDate { get; set; }
        public DateTime? IlrEffectiveToDate { get; set; }
                
        public decimal? IlrTotalCost { get; set; }

        public TriageStatusViewModel TriageStatusViewModel { get; set; }

        public DataLockErrorCode DataLockErrorCode { get; set; }
    }

    public enum TriageStatusViewModel
    {
        Unknown = 0,
        ChangeApprenticeship = 1,
        RestartApprenticeship = 2,
        FixInIlr = 3
    }
}