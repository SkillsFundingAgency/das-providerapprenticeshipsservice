using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations
{
    public static class FeatureToggleServiceRegistrations
    {
        public static IServiceCollection AddFeatureToggleService(this IServiceCollection services, IConfiguration configuration)
        {
            // replacing:
            // For<SFA.DAS.Authorization.ProviderFeatures.Configuration.ProviderFeaturesConfiguration>().Use(config.Features);
            // For<IFeatureTogglesService<DAS.Authorization.ProviderFeatures.Models.ProviderFeatureToggle>>().Use<FeatureTogglesService<DAS.Authorization.ProviderFeatures.Configuration.ProviderFeaturesConfiguration, DAS.Authorization.ProviderFeatures.Models.ProviderFeatureToggle>>();
            services.AddSingleton<SFA.DAS.Authorization.ProviderFeatures.Configuration.ProviderFeaturesConfiguration>(_ =>
            {
                var config = _.GetService<ProviderApprenticeshipsServiceConfiguration>();
                return config.Features;
            });

            services.AddTransient<IFeatureTogglesService<DAS.Authorization.ProviderFeatures.Models.ProviderFeatureToggle>, 
                FeatureTogglesService<DAS.Authorization.ProviderFeatures.Configuration.ProviderFeaturesConfiguration, DAS.Authorization.ProviderFeatures.Models.ProviderFeatureToggle>>();

            // DOUBT: There are two places where Features config is defined, WHY? WHICH ONE IS MEANT TO BE USED?
            // 1. ProviderApprenticeshipsServiceConfiguration - from SFA.DAS.Authorization.ProviderFeatures.Configuration.ProviderFeaturesConfiguration - this is not used!
            // 2. RoatpCourseManagementWebConfiguration (from SFA.DAS.Roatp.CourseManagement.Web) - this is used in GetRoatpBetaProviderService!

            return services;
        }
    }
}
