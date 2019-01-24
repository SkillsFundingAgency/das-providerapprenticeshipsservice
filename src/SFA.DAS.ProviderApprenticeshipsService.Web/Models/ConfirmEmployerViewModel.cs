using System.ComponentModel.DataAnnotations;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ConfirmEmployerViewModel : LegalEntityViewModel
    {
        public ConfirmEmployerViewModel()
        {
            
        }

        public ConfirmEmployerViewModel(LegalEntityViewModel legalEntityViewModel, EmployerSelectionAction? employerSelectionAction)
        {
            EmployerAccountPublicHashedId = legalEntityViewModel.EmployerAccountPublicHashedId;
            EmployerAccountName = legalEntityViewModel.EmployerAccountName;
            EmployerAccountLegalEntityPublicHashedId = legalEntityViewModel.EmployerAccountLegalEntityPublicHashedId;
            EmployerAccountLegalEntityName = legalEntityViewModel.EmployerAccountLegalEntityName;
            EmployerSelectionAction = employerSelectionAction;
        }

        [Required(ErrorMessage = "Please choose an option")]
        public bool? Confirm { get; set; }

        public EmployerSelectionAction? EmployerSelectionAction { get; set; }
    }
}