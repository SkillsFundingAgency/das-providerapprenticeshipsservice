using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface IApprovedApprenticeshipMapper
    {
        ApprovedApprenticeshipViewModel Map(ApprovedApprenticeship source);
    }
}