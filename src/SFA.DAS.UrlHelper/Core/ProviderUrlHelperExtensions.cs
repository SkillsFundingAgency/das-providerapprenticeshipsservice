#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using SFA.DAS.AutoConfiguration;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SFA.DAS.ProviderUrlHelper.Core
{
    public static class ProviderUrlHelperExtensions
    {
        private static ProviderUrlConfiguration _providerUrlConfiguration;

        public static string ProviderCommitmentsLink(this UrlHelperBase helper, string path)
        {
            var config = GetProviderUrlConfiguration(helper.ActionContext.HttpContext);

            return Action(config.ProviderCommitmentsBaseUrl, path);
        }

        public static string ProviderApprenticeshipServiceLink(this UrlHelperBase helper, string path)
        {
            var config = GetProviderUrlConfiguration(helper.ActionContext.HttpContext);

            return Action(config.ProviderApprenticeshipServiceBaseUrl, path);
        }

        private static ProviderUrlConfiguration GetProviderUrlConfiguration(HttpContext httpContext)
        {
            // Remarks: don't need to lock - last write wins.
            if (_providerUrlConfiguration == null)
            {
                var autoConfigurationService = ServiceLocator.Get<IAutoConfigurationService>(httpContext);
                _providerUrlConfiguration = autoConfigurationService.Get<ProviderUrlConfiguration>();
            }

            return _providerUrlConfiguration;
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl.TrimEnd('/');
            var trimmedPath = path.Trim('/');

            return $"{trimmedBaseUrl}/{trimmedPath}";
        }
    }
}
#endif