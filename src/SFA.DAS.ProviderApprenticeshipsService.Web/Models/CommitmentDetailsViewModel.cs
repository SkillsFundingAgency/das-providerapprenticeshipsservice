using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class CommitmentDetailsViewModel : ViewModelBase
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
        public bool RelationshipVerified { get; set; }

        public bool IsReadOnly { get; set; }
        public bool IsFundedByTransfer { get; set; }

        public Dictionary<string, string> Errors { get; set; }
        public Dictionary<string, string> Warnings { get; set; }
    }
}