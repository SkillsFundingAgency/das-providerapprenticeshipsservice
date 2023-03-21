using SFA.DAS.Http.Configuration;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    public interface IProviderAgreementStatusConfiguration : IBaseConfiguration
    {
        bool UseFakeIdentity { get; set; }
        string DatabaseConnectionString { get; set; }
        string ServiceBusConnectionString { get; set; }
        CommitmentsApiClientV2Configuration CommitmentsApiClientV2 { get; set; }
        NotificationsApiClientConfiguration NotificationApi { get; set; }
        ProviderNotificationConfiguration CommitmentNotification { get; set; }
        ProviderRelationshipsApiConfiguration ProviderRelationshipsApi { get; set; }
        string Hashstring { get; set; }
        int MaxBulkUploadFileSize { get; set; } // Size of file in kilobytes
        bool EnableEmailNotifications { get; set; }
        string AllowedHashstringCharacters { get; set; }
        string PublicHashstring { get; set; }
        string PublicAllowedHashstringCharacters { get; set; }
        string PublicAllowedAccountLegalEntityHashstringSalt { get; set; }
        string PublicAllowedAccountLegalEntityHashstringCharacters { get; set; }
        ContentClientApiConfiguration ContentApi { get; set; }
        string ContentApplicationId { get; set; }
        int DefaultCacheExpirationInMinutes { get; set; }
        ZenDeskConfiguration ZenDeskSettings { get; set; }
        SFA.DAS.Authorization.ProviderFeatures.Configuration.ProviderFeaturesConfiguration Features { get; set; }
    }

    public class ProviderApprenticeshipsServiceConfiguration : IProviderAgreementStatusConfiguration
    {
        public bool UseFakeIdentity { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public CommitmentsApiClientV2Configuration CommitmentsApiClientV2 { get; set; }
        public NotificationsApiClientConfiguration NotificationApi { get; set; }
        public ProviderNotificationConfiguration CommitmentNotification { get; set; }
        public ProviderRelationshipsApiConfiguration ProviderRelationshipsApi { get; set; }
        public string Hashstring { get; set; }
        public int MaxBulkUploadFileSize { get; set; } // Size of file in kilobytes
        public bool EnableEmailNotifications { get; set; }
        public string AllowedHashstringCharacters { get; set; }
        public string PublicHashstring { get; set; }
        public string PublicAllowedHashstringCharacters { get; set; }
        public string PublicAllowedAccountLegalEntityHashstringSalt { get; set; }
        public string PublicAllowedAccountLegalEntityHashstringCharacters { get; set; }
        public ContentClientApiConfiguration ContentApi { get; set; }
        public string ContentApplicationId { get; set; }
        public int DefaultCacheExpirationInMinutes { get; set; }
        public ZenDeskConfiguration ZenDeskSettings { get; set; }
        public SFA.DAS.Authorization.ProviderFeatures.Configuration.ProviderFeaturesConfiguration Features { get; set; }
    }

    public class CommitmentsApiClientV2Configuration : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }

    public interface IProviderNotificationConfiguration
    {
        public bool UseProviderEmail { get; set; }
        public bool SendEmail { get; set; }
        public string[] ProviderTestEmails { get; set; }
        public string IdamsListUsersUrl { get; set; }
        public string DasUserRoleId { get; set; }
        public string SuperUserRoleId { get; set; }
    }

    public class ProviderNotificationConfiguration : IProviderNotificationConfiguration, IJwtClientConfiguration
    {
        public bool UseProviderEmail { get; set; }
        public bool SendEmail { get; set; }
        public string[] ProviderTestEmails { get; set; }
        public string IdamsListUsersUrl { get; set; }
        public string DasUserRoleId { get; set; }
        public string SuperUserRoleId { get; set; }
        public string ClientToken { get; set; }
    }

    public class ContentClientApiConfiguration : IContentApiConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string IdentifierUri { get; set; }
    }

    public class ZenDeskConfiguration
    {
        public string SectionId { get; set; }
        public string SnippetKey { get; set; }
        public string CobrowsingSnippetKey { get; set; }
    }
}