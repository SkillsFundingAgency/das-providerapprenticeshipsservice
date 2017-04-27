using FluentValidation.Attributes;

using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock
{
    [Validator(typeof(ConfirmRestartViewModelValidator))]
    public class ConfirmRestartViewModel
    {
        public long ProviderId { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public bool? SendRequestToEmployer { get; set; }

        public long DataLockEventId { get; set; }

        public DataLockMismatchViewModel DataMismatchModel { get; set; }
    }
}