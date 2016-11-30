namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

    public sealed class FinishEditingViewModel
    {
        public long ProviderId { get; set; }
        public long CommitmentId { get; set; }

        [Required(ErrorMessage = "Select an option")]
        public SaveStatus SaveStatus { get; set; }

        public bool ApproveAndSend { get; set; }
    }
}