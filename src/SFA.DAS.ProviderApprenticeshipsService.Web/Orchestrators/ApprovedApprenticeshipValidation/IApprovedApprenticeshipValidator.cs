using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.ApprovedApprenticeshipValidation
{
    public interface IApprovedApprenticeshipValidator
    {
        Dictionary<string, string> Validate(ApprenticeshipViewModel model);
    }
}
