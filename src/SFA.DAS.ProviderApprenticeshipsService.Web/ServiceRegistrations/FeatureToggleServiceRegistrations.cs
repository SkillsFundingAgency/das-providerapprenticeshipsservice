using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

public static class FeatureToggleServiceRegistrations
{
    public static IServiceCollection AddFeatureToggleService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ProviderFeaturesConfiguration>(provider =>
        {
            var config = provider.GetService<ProviderApprenticeshipsServiceConfiguration>();
            return config.Features;
        });

        services.AddTransient<IFeatureTogglesService<ProviderFeatureToggle>, FeatureTogglesService<ProviderFeaturesConfiguration, ProviderFeatureToggle>>();

        // DOUBT: There are two places where Features config is defined, WHY? WHICH ONE IS MEANT TO BE USED?
        // 1. ProviderApprenticeshipsServiceConfiguration - from SFA.DAS.Authorization.ProviderFeatures.Configuration.ProviderFeaturesConfiguration - this is not used!
        // 2. RoatpCourseManagementWebConfiguration (from SFA.DAS.Roatp.CourseManagement.Web) - this is used in GetRoatpBetaProviderService!

        return services;
    }
}