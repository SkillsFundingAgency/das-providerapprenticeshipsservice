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

            switch (employerSelectionAction)
            {
                case Types.EmployerSelectionAction.CreateCohort:
                    Question = "Is this the employer you'd like to add a cohort for?";
                    ControllerName = "CreateCohort";
                    break;
                case Types.EmployerSelectionAction.CreateReservation:
                    Question = "Is this the employer you'd like to reserve funds for?";
                    ControllerName = "Reservation";
                    break;
            }
        }

        [Required(ErrorMessage = "Please choose an option")]
        public bool? Confirm { get; set; }

        public string Question { get; set; }
        public string ControllerName { get; set; }

        public EmployerSelectionAction? EmployerSelectionAction { get; set; }
    }
}