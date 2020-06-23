using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public interface IApprovedApprenticeshipValidator :  IApprenticeshipCoreValidator
    {
        Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel model);

        Dictionary<string, string> ValidateAcademicYear(CreateApprenticeshipUpdateViewModel dateTime);
    }
}
