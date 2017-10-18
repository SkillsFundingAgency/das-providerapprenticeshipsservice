using System.Collections.Generic;
using System.Linq;

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
        public bool ShowIlrDataMismatch { get; set; }

        public string DataLockSummaryTitle
        {
            get
            {
                var summary = string.Empty;

                if (ShowIlrDataMismatch)
                {
                    summary = "ILR data mismatch";

                    if (DataLockWithOnlyPriceMismatch != null && DataLockWithOnlyPriceMismatch.Any() && !ShowChangesRequested)
                    {
                        summary = "Price mismatch";
                    }
                    else if (DataLockWithOnlyPriceMismatch != null && DataLockWithOnlyPriceMismatch.Any() && ShowChangesRequested)
                    {
                        summary = "Price change request pending";
                    }
                    else if ((DataLockWithOnlyPriceMismatch != null && DataLockWithOnlyPriceMismatch.Any()) &&
                            (DataLockWithCourseMismatch != null && DataLockWithCourseMismatch.Any()))
                    {
                        summary = "Price and course mismatch";
                    }
                }

                return summary;

            }
        }

    }
}