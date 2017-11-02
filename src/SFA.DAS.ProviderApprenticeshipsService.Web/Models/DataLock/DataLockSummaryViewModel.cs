using System.Collections.Generic;
using System.Linq;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;

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

        public string DataLockSummaryTitle
        {
            get
            {
                var summary = string.Empty;

                if (ShowIlrDataMismatch)
                {
                    summary = "ILR data mismatch";
                    var courseAndPriceDataLockInOneDataLock = DataLockWithCourseMismatch?.Any(m => m.DataLockErrorCode.HasFlag(DataLockErrorCode.Dlock07) ) ?? false;
                    var courseAndPriceIn2DataLocks =    
                           (DataLockWithOnlyPriceMismatch?.Any() ?? false)
                        && (DataLockWithCourseMismatch?.Any()    ?? false);

                    if (courseAndPriceIn2DataLocks || courseAndPriceDataLockInOneDataLock)
                    {
                        summary = "Price and course mismatch";
                    }
                    else if (DataLockWithOnlyPriceMismatch?.Any() ?? false)
                    {
                        summary = "Price mismatch";
                    }
                    else if (DataLockWithCourseMismatch?.Any() ?? false)
                    {
                        summary = "Course mismatch";
                    }
                }

                return summary;
            }
        }
    }
}