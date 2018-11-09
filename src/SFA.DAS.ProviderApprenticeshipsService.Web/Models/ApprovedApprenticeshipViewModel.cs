using System;
using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;
using Boolean = DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle.Boolean;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprovedApprenticeshipViewModel : ViewModelBase
    {
        public string HashedApprenticeshipId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Uln { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? StopDate { get; set; }

        public string TrainingName { get; set; }

        public decimal CurrentCost { get; set; }

        public string Status { get; set; }

        public string EmployerName { get; set; }

        public PendingChanges PendingChanges { get; set; }

        public string ProviderReference { get; set; }

        public string CohortReference { get; set; }

        public string AccountLegalEntityPublicHashedId { get; set; }

        public bool EnableEdit { get; set; }

        //todo: this is not used in the manage apprentices list, so maybe create a new 
        //viewmodel for that page?
        public DataLockSummaryViewModel DataLockSummaryViewModel { get; set; }

        public bool PendingDataLockRestart { get; set; }

        public bool PendingDataLockChange { get; set; }

        public bool HasHadDataLockSuccess { get; set; }

        public ApprenticeshipFiltersViewModel SearchFiltersForListView { get; set; }

        public bool DataLockCourse { get; set; }
        public bool DataLockPrice { get; set; }
        public bool DataLockCourseTriaged { get; set; }
        public bool DataLockCourseChangeTriaged { get; set; }
        public bool DataLockPriceTriaged { get; set; }
    }
}