using System;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class CourseDataLockViewModel
    {
        public string TrainingName { get; set; }

        public DateTimeViewModel ApprenticeshipStartDate { get; set; }

        public string IlrTrainingName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime? IlrEffectiveFromDate { get; set; }

        public DateTime? IlrEffectiveToDate { get; set; }
    }
}