using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    public class ProviderApprenticeshipsServiceConfiguration : IProviderAgreementStatusConfiguration
    {
        public bool UseFakeIdentity { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public CommitmentsApiClientConfiguration CommitmentsApi { get; set; }
        public ApprenticeshipInfoServiceConfiguration ApprenticeshipInfoService { get; set; }
        public NotificationsApiClientConfiguration NotificationApi { get; set; }
        public string Hashstring { get; set; }
        public int MaxBulkUploadFileSize { get; set; } // Size of file in kilobytes
        public bool CheckForContractAgreements { get; set; }
        public string ContractAgreementsUrl { get; set; }
        public bool EnableEmailNotifications { get; set; }
        public string AllowedHashstringCharacters { get; set; }
        public string PublicHashstring { get; set; }
        public string PublicAllowedHashstringCharacters { get; set; }
        public string PublicAllowedAccountLegalEntityHashstringSalt { get; set; }
        public string PublicAllowedAccountLegalEntityHashstringCharacters { get; set; }
    }

    public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration, IJwtClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }

    public class ApprenticeshipInfoServiceConfiguration : IApprenticeshipInfoServiceConfiguration
    {
        public string BaseUrl { get; set; }
    }

    public class NotificationsApiClientConfiguration : INotificationsApiClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientToken { get; set; }


        public string ClientId { get; set; }
        public string ClientSecret { get; }
        public string IdentifierUri { get; }
        public string Tenant { get; }

        public string IdamsListUsersUrl { get; set; }
        public string DasUserRoleId { get; set; }
        public string SuperUserRoleId { get; set; }
    }
}