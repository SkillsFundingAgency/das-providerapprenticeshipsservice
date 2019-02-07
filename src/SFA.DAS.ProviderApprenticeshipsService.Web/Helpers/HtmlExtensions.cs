using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Helpers
{
    public static class HtmlExtensions
    {
        private readonly static Lazy<bool> _lazyIsEnabled = new Lazy<bool>(InitIsEnabled);
        private static bool InitIsEnabled()
        {
            var service = DependencyResolver.Current.GetService<IFeatureToggleService>();
            return service.Get<ManageReservations>().FeatureEnabled;
        }

        public static bool CanShowReservationsLink(this HtmlHelper htmlHelper)
        {
            var httpContext = DependencyResolver.Current.GetService<HttpContextBase>();
            var show = httpContext?.User?.Identity?.GetClaim(DasClaimTypes.ShowReservations);

            if(show?.ToLower() == "true" && _lazyIsEnabled.Value)
                return true;

            return false;
        }
    }
}