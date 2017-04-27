using FluentValidation.Attributes;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock
{
    [Validator(typeof(DataLockMismatchViewModelValidator))]
    public class DataLockMismatchViewModel
    {
        public DataLockViewModel DataLockViewModel { get; set; }

        public Apprenticeship DasApprenticeship { get; set; }

        public SubmitStatusViewModel? SubmitStatusViewModel { get; set; }

        public long ProviderId { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public long DataLockEventId { get; set; }
    }
}