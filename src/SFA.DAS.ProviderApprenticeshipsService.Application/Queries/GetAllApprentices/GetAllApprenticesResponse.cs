using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAllApprentices
{
    public class GetAllApprenticesResponse
    {
        public List<Apprenticeship> Apprenticeships { get; set; }
    }
}