using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ManageApprenticeshipsViewModel
    {
        // ToDo: use list view model
        public IEnumerable<ApprenticeshipDetailsViewModel> Apprenticeships { get; set; } 

        public long ProviderId { get; set; }
    }
}