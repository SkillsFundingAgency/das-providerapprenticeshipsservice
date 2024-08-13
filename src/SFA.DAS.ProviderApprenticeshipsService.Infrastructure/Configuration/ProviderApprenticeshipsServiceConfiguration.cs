using SFA.DAS.Authorization.ProviderFeatures.Configuration;
using SFA.DAS.Http.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

public class ProviderApprenticeshipsServiceConfiguration : IBaseConfiguration
{
    public bool UseFakeIdentity { get; set; }
    public string DatabaseConnectionString { get; set; }
    public string ServiceBusConnectionString { get; set; }
    public string NServiceBusLicense { get; set; }
    public CommitmentsApiClientV2Configuration CommitmentsApiClientV2 { get; set; }
    public TrainingProviderApiClientConfiguration TrainingProviderApiClientConfiguration { get; set; }
    public ProviderNotificationConfiguration CommitmentNotification { get; set; }
    public ProviderRelationshipsApiConfiguration ProviderRelationshipsApi { get; set; }
    public int MaxBulkUploadFileSize { get; set; } // Size of file in kilobytes
    public ContentClientApiConfiguration ContentApi { get; set; }
    public string ContentApplicationId { get; set; }
    public int DefaultCacheExpirationInMinutes { get; set; }
    public ZenDeskConfiguration ZenDeskSettings { get; set; }
    public ProviderFeaturesConfiguration Features { get; set; }
    /// <summary>
    /// Gets or Sets property UseDfESignIn.
    /// Property responsible for holding the DfESignIn toggle switch value.
    /// </summary>
    public bool UseDfESignIn { get; set; } = false;

    public string DataProtectionKeysDatabase { get; set; }
    public string RedisConnectionString { get; set; }
    public string TraineeshipCutOffDate { get; set; }
}

public class CommitmentsApiClientV2Configuration : IManagedIdentityClientConfiguration
{
    public string ApiBaseUrl { get; set; }
    public string IdentifierUri { get; set; }
}

public record TrainingProviderApiClientConfiguration : IManagedIdentityClientConfiguration
{
    public string ApiBaseUrl { get; init; }
    public string IdentifierUri { get; init; }
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