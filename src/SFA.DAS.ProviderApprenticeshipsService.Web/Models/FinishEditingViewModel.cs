using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class FinishEditingViewModel
    {
        public string ProviderId { get; set; }
        public string CommitmentId { get; set; }

        [Required(ErrorMessage = "Select an option")]
        public string SaveOrSend { get; set; }
    }
}