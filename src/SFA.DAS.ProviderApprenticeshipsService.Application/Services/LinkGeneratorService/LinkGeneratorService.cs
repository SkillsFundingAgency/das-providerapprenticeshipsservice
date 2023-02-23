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

        public virtual string ProviderCommitmentsLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.ProviderCommitmentsBaseUrl;
            return Action(baseUrl, path);
        }

        public string ProviderApprenticeshipServiceLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.ProviderApprenticeshipServiceBaseUrl;
            return Action(baseUrl, path);
        }

        public string ReservationsLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.ReservationsBaseUrl;
            return Action(baseUrl, path);
        }

        public string RecruitLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.RecruitBaseUrl;
            return Action(baseUrl, path);
        }

        public string TraineeshipLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.TraineeshipBaseUrl;
            return Action(baseUrl, path);
        }

        public string RegistrationLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.RegistrationBaseUrl;
            return Action(baseUrl, path);
        }

        public string EmployerDemandLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.EmployerDemandBaseUrl;
            return Action(baseUrl, path);
        }

        public string CourseManagementLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.CourseManagementBaseUrl;
            return Action(baseUrl, path);
        }

        public string ProviderFundingLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.ProviderFundingBaseUrl;
            return Action(baseUrl, path);
        }

        public string APIManagementLink(string path)
        {
            var baseUrl = _providerUrlConfiguration.APIManagementBaseUrl;
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