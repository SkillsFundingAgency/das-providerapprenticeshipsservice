using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAllApprentices
{
    public class GetAllApprenticesResponse
    {
        public List<Apprenticeship> Apprenticeships { get; set; }
    }
}