using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class CommitmentListViewModel
    {
        public long ProviderId { get; set; }
        public List<CommitmentListItemViewModel> Commitments { get; set; }

        public bool HasSignedAgreement { get; set; }
    }
}