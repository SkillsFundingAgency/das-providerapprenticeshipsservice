using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock
{
    public class DataLockSummaryViewModel
    {
        public List<DataLockViewModel> DataLockWithCourseMismatch { get; set; }
        public List<DataLockViewModel> DataLockWithOnlyPriceMismatch { get; set; }

        public bool ShowChangesRequested { get; set; }
        public bool ShowChangesPending { get; set; }

    }
}