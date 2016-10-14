using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks
{
    public class GetFrameworksQueryResponse
    {
        public List<Framework> Frameworks { get; set; } 
    }
}