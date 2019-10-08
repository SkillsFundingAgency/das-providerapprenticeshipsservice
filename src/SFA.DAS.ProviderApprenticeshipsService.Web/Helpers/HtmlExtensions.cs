using System.Web;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Helpers
{
    public static class HtmlExtensions
    {
        public static bool CanShowReservationsLink(this HtmlHelper htmlHelper)
        {
            var httpContext = DependencyResolver.Current.GetService<HttpContextBase>();
            var show = httpContext?.User?.Identity?.GetClaim(DasClaimTypes.ShowReservations);
            return show?.ToLower() == "true";
        }
    }
}