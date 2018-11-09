using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface IAlertMapper
    {
        List<string> MapAlerts(Apprenticeship apprenticeship);
        List<string> MapAlerts(ApprovedApprenticeshipViewModel model, ApprovedApprenticeship apprenticeship);
    }
}