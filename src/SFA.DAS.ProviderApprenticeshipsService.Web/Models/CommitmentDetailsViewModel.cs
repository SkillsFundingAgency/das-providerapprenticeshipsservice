using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class CommitmentDetailsViewModel
    {
        public long ProviderId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string LegalEntityName { get; set; }
        public string Reference { get; set; }
        public RequestStatus Status { get; set; }
        public bool HasApprenticeships { get; set; }
        public IList<ApprenticeshipListItemViewModel> Apprenticeships { get; set; }
        public IList<ApprenticeshipListItemGroupViewModel> ApprenticeshipGroups { get; set; }
        public string LatestMessage { get; set; }
        public bool PendingChanges { get; set; }

        public string BackLinkUrl { get; set; }
    }
}