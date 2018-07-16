using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards
{
    public class GetStandardsQueryResponse
    {
        public List<Standard> Standards { get; set; }
    }
}