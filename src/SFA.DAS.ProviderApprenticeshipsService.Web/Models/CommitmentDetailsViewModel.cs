using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class CommitmentDetailsViewModel
    {
        public long ProviderId { get; set; }
        public long CommitmentId { get; set; }
        public string LegalEntityName { get; set; }
        public string Reference { get; set; }
        public RequestStatus Status { get; set; }
        public IList<ApprenticeshipListItemViewModel> Apprenticeships { get; set; }
        public string LatestMessage { get; set; }
        public bool PendingChanges { get; set; }
    }
}