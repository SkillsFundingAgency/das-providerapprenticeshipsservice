using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface ISelectEmployerMapper
    {
        ChooseEmployerViewModel Map(IEnumerable<AccountProviderLegalEntityDto> source, EmployerSelectionAction action = EmployerSelectionAction.CreateCohort);
    }
}