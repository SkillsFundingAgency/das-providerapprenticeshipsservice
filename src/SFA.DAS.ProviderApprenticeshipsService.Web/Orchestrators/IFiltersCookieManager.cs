using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public interface IFiltersCookieManager
    {
        ApprenticeshipFiltersViewModel CheckForCookie(ApprenticeshipFiltersViewModel filtersViewModel);
    }
}