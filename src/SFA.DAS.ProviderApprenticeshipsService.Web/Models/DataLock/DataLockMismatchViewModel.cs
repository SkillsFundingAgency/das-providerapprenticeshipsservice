using FluentValidation.Attributes;
using System.Collections.Generic;
using System.Linq;

using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock
{
    [Validator(typeof(DataLockMismatchViewModelValidator))]
    public class DataLockMismatchViewModel
    {
        public DataLockSummaryViewModel DataLockSummaryViewModel { get; set; }

        public ApprenticeshipViewModel DasApprenticeship { get; set; }

        public SubmitStatusViewModel? SubmitStatusViewModel { get; set; }

        public long ProviderId { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public string EmployerName { get; set; }

        public IEnumerable<PriceHistoryViewModel> PriceDataLocks { get; set; }

        public IEnumerable<CourseDataLockViewModel> CourseDataLocks { get; set; }

        public int TotalChanges => (PriceDataLocks?.Count() ?? 0) + (CourseDataLocks?.Count() ?? 0);
    }
}