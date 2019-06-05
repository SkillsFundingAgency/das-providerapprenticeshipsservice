using System;

namespace SFA.DAS.ProviderUrlHelper
{
    public class LinkGenerator : ILinkGenerator
    {
        private readonly ProviderUrlConfiguration _configuration;

        public LinkGenerator(ProviderUrlConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ProviderCommitmentsLink(string path)
        {
            var baseUrl = _configuration.ProviderCommitmentsBaseUrl;
            return Action(baseUrl, path);
        }

        public string ProviderApprenticeshipServiceLink(string path)
        {
            var baseUrl = _configuration.ProviderApprenticeshipServiceBaseUrl;
            return Action(baseUrl, path);
        }

        public string ReservationsLink(string path)
        {
            var baseUrl = _configuration.ReservationsBaseUrl;
            return Action(baseUrl, path);
        }

        public string RecruitLink(string path)
        {
            var baseUrl = _configuration.RecruitBaseUrl;
            return Action(baseUrl, path);
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl.TrimEnd('/');
            var trimmedPath = path.Trim('/');

            return $"{trimmedBaseUrl}/{trimmedPath}";
        }
    }
}