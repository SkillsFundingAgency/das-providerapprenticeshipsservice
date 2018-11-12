using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.CreateCohort
{
    public class ConfirmEmployerViewModel : LegalEntityViewModel
    {
        [Required(ErrorMessage = "Please choose an option")]
        public bool? Confirm { get; set; }
    }
}