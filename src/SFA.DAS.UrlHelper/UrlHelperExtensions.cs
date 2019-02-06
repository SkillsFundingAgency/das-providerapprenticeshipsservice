using System;
using SFA.DAS.AutoConfiguration;
#if NETFRAMEWORK
using System.Web.Mvc;
using UrlHelper=System.Web.Mvc.UrlHelper;
#elif NETCOREAPP
using UrlHelper=Microsoft.AspNetCore.Mvc.Routing .UrlHelper;
#endif

namespace SFA.DAS.ProviderUrlHelper
{
    public static class UrlHelperExtensions
    {
        private static readonly Lazy<ProviderUrlConfiguration> LazyProviderConfiguration = new Lazy<ProviderUrlConfiguration>(LoadProviderUrlConfiguration);

        public static string ProviderCommitmentsLink(this UrlHelper helper, string path)
        {
            var configuration = LazyProviderConfiguration.Value;
            var baseUrl = configuration.ProviderCommitmentsBaseUrl;

            return Action(baseUrl, path);
        }

        public static string ProviderApprenticeshipServiceLink(this UrlHelper helper, string path)
        {
            var configuration = LazyProviderConfiguration.Value;
            var baseUrl = configuration.ProviderApprenticeshipServiceBaseUrl;

            return Action(baseUrl, path);
        }

        private static ProviderUrlConfiguration LoadProviderUrlConfiguration()
        {
            var autoConfigurationService = ServiceLocator.Get<IAutoConfigurationService>();

            var configuration = autoConfigurationService.Get<ProviderUrlConfiguration>();

            return configuration;
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl.TrimEnd('/');

            return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
        }
    }
}
