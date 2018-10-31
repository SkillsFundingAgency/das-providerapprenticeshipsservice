using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.CreateCohort
{
    public class CreateCohortViewModel
    {
        public IEnumerable<LegalEntityViewModel> LegalEntities { get; set; }

        public CreateCohortViewModel()
        {
            LegalEntities = new List<LegalEntityViewModel>();
        }
    }
}