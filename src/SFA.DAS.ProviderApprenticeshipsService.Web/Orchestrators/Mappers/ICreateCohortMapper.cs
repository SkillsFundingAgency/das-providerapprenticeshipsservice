using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface ICreateCohortMapper
    {
        ChooseEmployerViewModel Map(IEnumerable<AccountProviderLegalEntityDto> source);
    }
}