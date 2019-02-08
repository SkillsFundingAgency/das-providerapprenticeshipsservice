#if NETFRAMEWORK
using System;
using SFA.DAS.AutoConfiguration;
using UrlHelper=System.Web.Mvc.UrlHelper;

namespace SFA.DAS.ProviderUrlHelper.Framework
{
    public static class ProviderUrlHelperExtensions
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
            var trimmedPath = path.Trim('/');

            return $"{trimmedBaseUrl}/{trimmedPath}";
        }
    }
}
#endif
