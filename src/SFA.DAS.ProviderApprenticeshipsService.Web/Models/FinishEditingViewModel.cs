using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class FinishEditingViewModel
    {
        public long ProviderId { get; set; }
        public long CommitmentId { get; set; }

        [Required(ErrorMessage = "Select an option")]
        public string SaveOrSend { get; set; }
    }
}