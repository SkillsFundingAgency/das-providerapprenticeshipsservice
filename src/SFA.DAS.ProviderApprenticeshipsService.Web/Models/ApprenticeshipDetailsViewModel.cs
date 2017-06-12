using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprenticeshipDetailsViewModel
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

        public bool HasDataLockError { get; set; }

        public DataLockErrorType ErrorType { get; set; }

        public string RecordStatus { get; set; }

        public string DataLockStatus { get; set; }

        public bool HasRequestedRestart { get; set; }

        //todo: this is not used in the manage apprentices list, so maybe create a new 
        //viewmodel for that page?
        public List<DataLockViewModel> DataLocks { get; set; }

        public bool HasCourseDataLockMismatches
        {
            get
            {
                return DataLocks.Any(x =>
                        x.DataLockErrorCode.HasFlag(DataLockErrorCode.Dlock03)
                        || x.DataLockErrorCode.HasFlag(DataLockErrorCode.Dlock04)
                        || x.DataLockErrorCode.HasFlag(DataLockErrorCode.Dlock05)
                        || x.DataLockErrorCode.HasFlag(DataLockErrorCode.Dlock06));
            }
        }

        public bool HasPriceOnlyDataLockMismatches
        {
            get { return DataLocks.Any(x => x.DataLockErrorCode == DataLockErrorCode.Dlock07); }
        }
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