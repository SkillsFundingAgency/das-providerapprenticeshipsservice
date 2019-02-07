using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Helpers
{
    public static class HtmlExtensions
    {
        public static bool IsManageReservationsEnabled(this HtmlHelper htmlHelper, long providerId)
        {
            var service = DependencyResolver.Current.GetService<IFeatureToggleService>();
            var isEnabled = service.Get<ManageReservations>().FeatureEnabled;
            return isEnabled;
        }

        public static bool CanShowReservationsLink(this HtmlHelper htmlHelper)
        {
            var httpContext = DependencyResolver.Current.GetService<HttpContextBase>();
            var show = httpContext?.User?.Identity?.GetClaim(DasClaimTypes.ShowReservations);

            if(show?.ToLower() == "true")
                return true;

            return false;
        }
    }
}