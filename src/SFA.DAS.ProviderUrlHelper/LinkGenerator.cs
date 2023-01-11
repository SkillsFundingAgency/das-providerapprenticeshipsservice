using System;
using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.ProviderUrlHelper
{
    public class LinkGenerator : ILinkGenerator
    {
        private readonly Lazy<ProviderUrlConfiguration> _lazyProviderConfiguration;

        public LinkGenerator(IAutoConfigurationService autoConfigurationService)
        {
            _lazyProviderConfiguration =
                new Lazy<ProviderUrlConfiguration>(() => LoadProviderUrlConfiguration(autoConfigurationService));
        }

        public virtual string ProviderCommitmentsLink(string path)
        {
            var configuration = _lazyProviderConfiguration.Value;
            var baseUrl = configuration.ProviderCommitmentsBaseUrl;

            return Action(baseUrl, path);
        }

        public string ProviderApprenticeshipServiceLink(string path)
        {
            var configuration = _lazyProviderConfiguration.Value;
            var baseUrl = configuration.ProviderApprenticeshipServiceBaseUrl;

            return Action(baseUrl, path);
        }

        public string ReservationsLink(string path)
        {
            var configuration = _lazyProviderConfiguration.Value;
            var baseUrl = configuration.ReservationsBaseUrl;

            return Action(baseUrl, path);
        }

        public string RecruitLink(string path)
        {
            var configuration = _lazyProviderConfiguration.Value;
            var baseUrl = configuration.RecruitBaseUrl;

            return Action(baseUrl, path);
        }
        
        public string TraineeshipLink(string path)
        {
            var configuration = _lazyProviderConfiguration.Value;
            var baseUrl = configuration.TraineeshipBaseUrl;

            return Action(baseUrl, path);
        }

        public string RegistrationLink(string path)
        {
            var configuration = _lazyProviderConfiguration.Value;
            var baseUrl = configuration.RegistrationBaseUrl;

            return Action(baseUrl, path);
        }

        public string EmployerDemandLink(string path)
        {
            var configuration = _lazyProviderConfiguration.Value;
            var baseUrl = configuration.EmployerDemandBaseUrl;
            return Action(baseUrl, path);
        }

        public string CourseManagementLink(string path)
        {
            var configuration = _lazyProviderConfiguration.Value;
            var baseUrl = configuration.CourseManagementBaseUrl;
            return Action(baseUrl, path);
        }

        public string ProviderFundingLink(string path)
        {
            var configuration = _lazyProviderConfiguration.Value;
            var baseUrl = configuration.ProviderFundingBaseUrl;
		}

        public string APIManagementLink(string path)
        {
            var configuration = _lazyProviderConfiguration.Value;
            var baseUrl = configuration.APIManagementBaseUrl;
            return Action(baseUrl, path);
        }

        private ProviderUrlConfiguration LoadProviderUrlConfiguration(IAutoConfigurationService autoConfigurationService)
        {
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