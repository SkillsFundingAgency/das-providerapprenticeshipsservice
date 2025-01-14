using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAccountApiClient(configuration);
        services.AddTrainingProviderApi(configuration);
        services.AddDataRepositories();
        services.AddCommitmentsV2ApiClient(configuration);
        services.AddContentApi(configuration);
        services.AddProviderPRWebConfiguration(configuration);
        services.AddTransient<IBackgroundNotificationService, BackgroundNotificationService>();
        services.AddValidators();
        services.AddUserIdentityService();

        services.AddCookieStorageService();
        services.AddLinkGenerator();
        services.AddTransient<ICurrentDateTime, CurrentDateTime>();

        // the below is closely tied to Web, so not sure where logically best to register them
        services.AddTransient<IProviderCommitmentsLogger, ProviderCommitmentsLogger>(); // need to think where to inject it > move to Application
        services.AddScoped<IHtmlHelpers, HtmlHelpers>(); // to be grouped to somewhere else

        return services;
    }
}