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
        private static readonly Lazy<bool> _lazyIsEnabled = new Lazy<bool>(InitIsEnabled);

        public static bool CanShowReservationsLink(this HtmlHelper htmlHelper)
        {
            if (_lazyIsEnabled.Value)
            {
                var httpContext = DependencyResolver.Current.GetService<HttpContextBase>();
                var show = httpContext?.User?.Identity?.GetClaim(DasClaimTypes.ShowReservations);

                if (show?.ToLower() == "true")
                    return true;
            }
            return false;
        }

        private static bool InitIsEnabled()
        {
            var service = DependencyResolver.Current.GetService<IFeatureToggleService>();
            return service.Get<ManageReservations>().FeatureEnabled;
        }
    }
}