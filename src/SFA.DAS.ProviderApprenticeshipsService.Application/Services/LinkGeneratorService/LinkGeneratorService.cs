using SFA.DAS.AutoConfiguration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services.LinkGeneratorService
{
    public class LinkGeneratorService : ILinkGeneratorService
    {
        private readonly ProviderUrlConfiguration _providerUrlConfiguration;

        public LinkGeneratorService(ProviderUrlConfiguration providerUrlConfiguration)
        {
            _providerUrlConfiguration = providerUrlConfiguration;
        }

        public string TraineeshipLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.TraineeshipBaseUrl;
            return Action(baseUrl, path);
        }

        public string ProviderFundingLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.ProviderFundingBaseUrl;
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