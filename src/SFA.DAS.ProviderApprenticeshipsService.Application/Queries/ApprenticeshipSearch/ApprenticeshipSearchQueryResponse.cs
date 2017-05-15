using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch
{
    public class ApprenticeshipSearchQueryResponse
    {
        public List<Apprenticeship> Apprenticeships { get; set; }
        public Facets Facets { get; set; }
    }
}
