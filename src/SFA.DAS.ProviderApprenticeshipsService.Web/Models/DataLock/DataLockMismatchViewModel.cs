using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock
{
    public class DataLockMismatchViewModel
    {
        public DataLockViewModel DataLockViewModel { get; set; }

        public Apprenticeship DasApprenticeship { get; set; }

        public SubmitStatus? SubmitStatus { get; set; }

        public long ProviderId { get; set; }

        public string HashedApprenticeshipId { get; set; }
    }
}