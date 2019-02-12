#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SFA.DAS.ProviderUrlHelper.Core
{
    public static class ProviderUrlHelperExtensions
    {
        public static string ProviderCommitmentsLink(this UrlHelperBase helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.ProviderCommitmentsLink(path);
        }

        public static string ProviderApprenticeshipServiceLink(this UrlHelperBase helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.ProviderApprenticeshipServiceLink(path);

        }

        public static string ReservationsLink(this UrlHelperBase helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.ReservationsLink(path);
        }

        private static ILinkGenerator GetLinkGenerator(HttpContext httpContext)
        {
            return ServiceLocator.Get<ILinkGenerator>(httpContext);
        }
    }
}
#endif