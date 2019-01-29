using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Helpers
{
    public static class FeatureEnabledCheck
    {
        public static bool IsManageReservationsEnabled(this HtmlHelper htmlHelper, long providerId)
        {
            var service = DependencyResolver.Current
                .GetService<IFeatureToggleService>();
            var isEnabled = service.Get<Domain.Models.FeatureToggles.ManageReservations>().FeatureEnabled;
            return isEnabled;
        }
    }
}