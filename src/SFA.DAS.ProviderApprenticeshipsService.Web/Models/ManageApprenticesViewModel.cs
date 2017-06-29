using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ManageApprenticeshipsViewModel
    {
        public IEnumerable<ApprenticeshipDetailsViewModel> Apprenticeships { get; set; } 

        public ApprenticeshipFiltersViewModel Filters { get; set; }

        public int TotalApprenticeships { get; set; }

        public long ProviderId { get; set; }

        public int PageNumber { get; internal set; }

        public int TotalPages { get; internal set; }
        public int PageSize { get; internal set; }
    }
}