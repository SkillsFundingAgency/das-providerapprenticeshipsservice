using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

public static class FeatureToggleServiceRegistrations
{
    public static IServiceCollection AddProviderFeatures(this IServiceCollection services)
    {
        services.AddSingleton<ProviderFeaturesConfiguration>(provider =>
        {
            var config = provider.GetService<ProviderApprenticeshipsServiceConfiguration>();
            return config.Features;
        });

        services.AddTransient<IAuthorizationHandler, ProviderFeaturesAuthorizationHandler>();
        services.AddTransient<IFeatureTogglesService<ProviderFeatureToggle>, FeatureTogglesService<ProviderFeaturesConfiguration, ProviderFeatureToggle>>();
        
        return services;
    }
}