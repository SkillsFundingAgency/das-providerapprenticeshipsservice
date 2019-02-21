using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ChooseEmployerViewModel
    {
        [Required(ErrorMessage = "Please choose an option")]
        public EmployerSelectionAction? EmployerSelectionAction { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ControllerName { get; set; }

        public IEnumerable<LegalEntityViewModel> LegalEntities { get; set; }

        public ChooseEmployerViewModel()
        {
            LegalEntities = new List<LegalEntityViewModel>();
        }
    }
}