using System.Collections.Generic;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.ApprovedApprenticeshipValidation
{
    public interface IApprovedApprenticeshipValidator
    {
        Dictionary<string, string> Validate(ApprenticeshipViewModel model);

        Dictionary<string, string> ValidateAcademicYear(CreateApprenticeshipUpdateViewModel dateTime);
    }
}
