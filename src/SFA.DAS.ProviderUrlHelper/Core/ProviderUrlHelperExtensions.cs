#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.ProviderUrlHelper.Core
{
    public static class ProviderUrlHelperExtensions
    {
        public static string ProviderCommitmentsLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.ProviderCommitmentsLink(path);
        }

        public static string ProviderApprenticeshipServiceLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.ProviderApprenticeshipServiceLink(path);

        }

        public static string ReservationsLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.ReservationsLink(path);
        }

        public static string RecruitLink(this IUrlHelper helper, string path)
        {
            var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

            return linkGenerator.RecruitLink(path);
        }

        private static ILinkGenerator GetLinkGenerator(HttpContext httpContext)
        {
            return ServiceLocator.Get<ILinkGenerator>(httpContext);
        }
    }
}
#endif