using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks
{
    public class GetFrameworksQueryResponse
    {
        public List<Framework> Frameworks { get; set; } 
    }
}