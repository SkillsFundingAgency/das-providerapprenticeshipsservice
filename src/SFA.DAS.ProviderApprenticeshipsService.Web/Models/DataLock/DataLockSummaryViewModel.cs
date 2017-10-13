using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock
{
    public class DataLockSummaryViewModel
    {
        public List<DataLockViewModel> DataLockWithCourseMismatch { get; set; }
        public List<DataLockViewModel> DataLockWithOnlyPriceMismatch { get; set; }

        public bool ShowChangesRequested { get; set; }
        public bool ShowChangesPending { get; set; }

        public bool ShowCourseDataLockTriageLink { get; set; }
        public bool ShowPriceDataLockTriageLink { get; set; }

        public bool ShowIlrDataMismatch => ShowCourseDataLockTriageLink || ShowPriceDataLockTriageLink;

        public bool AnyTriagedDatalocks => ShowChangesPending || ShowChangesRequested;
    }
}