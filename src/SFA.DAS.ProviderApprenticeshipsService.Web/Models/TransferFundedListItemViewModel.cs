using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class TransferFundedListItemViewModel
    {
        public string HashedCommitmentId { get; set; }
        public string ReceivingEmployerName { get; set; }
        public TransferApprovalStatus? Status { get; set; }
    }
}