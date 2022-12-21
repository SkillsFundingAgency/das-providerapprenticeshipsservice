using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
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
        public bool CheckForContractAgreements { get; set; }
        public string ContractAgreementsUrl { get; set; }
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
        
        /// <summary>
        /// Gets or Sets property UseDfESignIn.
        /// Property responsible for holding the DfESignIn toggle switch value.
        /// </summary>
        public bool UseDfESignIn { get; set; } = false; 
    }

    public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
        public string ApiBaseUrl { get; }
        public string Tenant { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string IdentifierUri { get; }
    }

    public class CommitmentsApiClientV2Configuration : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }

    public class ProviderNotificationConfiguration : IJwtClientConfiguration
    {
        public bool SendEmail { get; set; }

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

    public class RoatpCourseManagementWebConfiguration : IRoatpCourseManagementWebConfiguration
    {
        public ProviderFeaturesConfiguration ProviderFeaturesConfiguration { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
    public class ProviderFeaturesConfiguration
    {
        public List<ProviderFeatureToggle> FeatureToggles { get; set; }
    }

    public class ProviderFeatureToggle : FeatureToggle
    {
        public List<ProviderFeatureToggleWhitelistItem> Whitelist { get; set; }
    }

    public class FeatureToggle
    {
        public string Feature { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class ProviderFeatureToggleWhitelistItem
    {
        public int Ukprn { get; set; }
    }
}