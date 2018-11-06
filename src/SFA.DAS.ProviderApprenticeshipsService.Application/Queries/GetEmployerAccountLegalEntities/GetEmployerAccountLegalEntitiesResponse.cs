using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Organisation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmployerAccountLegalEntities
{
    public class GetEmployerAccountLegalEntitiesResponse
    {
        public List<LegalEntity> LegalEntities { get; set; }
    }
}
