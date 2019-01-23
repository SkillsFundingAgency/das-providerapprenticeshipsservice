using System.ComponentModel.DataAnnotations;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ConfirmEmployerViewModel : LegalEntityViewModel
    {
        [Required(ErrorMessage = "Please choose an option")]
        public bool? Confirm { get; set; }

        [Required(ErrorMessage = "Please choose an option")]
        public EmployerSelectionAction? EmployerSelectionAction { get; set; }
    }
}