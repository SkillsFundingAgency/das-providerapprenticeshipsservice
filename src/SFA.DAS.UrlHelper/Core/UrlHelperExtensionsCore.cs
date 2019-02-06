using Microsoft.AspNetCore.Http;
using SFA.DAS.AutoConfiguration;
using UrlHelper=Microsoft.AspNetCore.Mvc.Routing .UrlHelper;

namespace SFA.DAS.ProviderUrlHelper.Core
{
    public static class UrlHelperExtensions
    {
        private static ProviderUrlConfiguration _providerUrlConfiguration;

        public static string ProviderCommitmentsLink(this UrlHelper helper, string path)
        {
            var config = GetProviderUrlConfiguration(helper.ActionContext.HttpContext);

            return Action(config.ProviderCommitmentsBaseUrl, path);
        }

        public static string ProviderApprenticeshipServiceLink(this UrlHelper helper, string path)
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

            return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
        }
    }
}
