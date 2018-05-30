using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Extensions
{
    /// <remarks>
    /// Intuitively it feels this shouldn't be duplicating CommitmentListItemExtensions.
    /// Perhaps CommitmentView should derive from CommitmentListItem?
    /// </remarks>
    public static class CommitmentViewExtensions
    {
        private static readonly CommitmentStatusCalculator StatusCalculator = new CommitmentStatusCalculator();

        public static RequestStatus GetStatus(this CommitmentView commitment)
        {
            return StatusCalculator.GetStatus(
                commitment.EditStatus,
                commitment.Apprenticeships.Count,
                commitment.LastAction,
                commitment.AgreementStatus,
                commitment.ProviderLastUpdateInfo,
                commitment.TransferSender?.Id,
                commitment.TransferSender?.TransferApprovalStatus);
        }

        public static bool IsTransfer(this CommitmentView commitment)
        {
            return commitment.TransferSender != null;
        }
    }
}
