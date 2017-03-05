using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Tasks.Api.Client.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    public class ProviderApprenticeshipsServiceConfiguration : IConfiguration
    {
        public bool UseFakeIdentity { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public CommitmentsApiClientConfiguration CommitmentsApi { get; set; }
        public TasksApiClientConfiguration TasksApi { get; set; }
        public ApprenticeshipInfoServiceConfiguration ApprenticeshipInfoService { get; set; }
        public NotificationsApiClientConfiguration NotificationApi { get; set; }
        public string Hashstring { get; set; }
        public int MaxBulkUploadFileSize { get; set; } // Size of file in kilobytes
        public bool CheckForContractAgreements { get; set; }
        public string ContractAgreementsUrl { get; set; }
        public bool EnableEmailNotifications { get; set; }
    }

    public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }

    public class TasksApiClientConfiguration : ITasksApiClientConfiguration
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
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}