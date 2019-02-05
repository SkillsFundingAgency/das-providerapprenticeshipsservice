//#define NET462
using System;
using System.Web.Mvc;
using SFA.DAS.AutoConfiguration;
#if NET462
using UrlHelper=System.Web.Mvc.UrlHelper;
#else
using UrlHelper =Microsoft.AspNetCore.Mvc.Routing.UrlHelper;
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
            var autoConfigurationService = DependencyResolver.Current.GetService<IAutoConfigurationService>();

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
