using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class FinishEditingViewModel
    {
        public long ProviderId { get; set; }
        public long CommitmentId { get; set; }

        [Required(ErrorMessage = "Select an option")]
        public SaveOrSendStatus SaveOrSendStatus { get; set; }

        public bool ApproveAndSend { get; set; }
    }

    public enum SaveOrSendStatus
    {
        // ToDo: Move to file
        Save = 0,
        Approve = 1,
        ApproveAndSend = 2,
        AmendAndSend = 3
    }
}