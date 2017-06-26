using System;
using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprenticeshipDetailsViewModel : ViewModelBase
    {
        public string HashedApprenticeshipId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Uln { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TrainingName { get; set; }

        public decimal? Cost { get; set; }

        public string Status { get; set; }

        public string EmployerName { get; set; }

        public PendingChanges PendingChanges { get; set; }
        
        public string ProviderReference { get; set; }

        public string CohortReference { get; set; }

        public bool EnableEdit { get; set; }

        public List<string> Alerts { get; set; }

        //todo: this is not used in the manage apprentices list, so maybe create a new 
        //viewmodel for that page?
        public DataLockSummaryViewModel DataLockSummaryViewModel { get; set; }
    }

    public enum DataLockErrorType
    {
        None = 0,
        RestartRequired = 1,
        UpdateNeeded = 2
    }

    public enum PendingChanges
    {
        None = 0,
        ReadyForApproval = 1,
        WaitingForEmployer = 2
    }
}