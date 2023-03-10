using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.GetRoatpBetaProviderService;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class RoatpCourseManagementWebServiceRegistrations
    {
        public static IServiceCollection AddRoatpServices(this IServiceCollection services, IConfiguration configuration)
        {
            // services.AddSingleton<IRoatpCourseManagementWebConfiguration>(configuration.Get<RoatpCourseManagementWebConfiguration>());
            // services.AddSingleton<IRoatpCourseManagementWebConfiguration>(cfg => cfg.GetService<IOptions<RoatpCourseManagementWebConfiguration>>().Value);
            services.AddSingleton(configuration.Get<RoatpCourseManagementWebConfiguration>());

            services.AddTransient<IGetRoatpBetaProviderService, GetRoatpBetaProviderService>();

            return services;
        }
        // REPLCAING:
        // For<IRoatpCourseManagementWebConfiguration>().Use(configurationService.Get<RoatpCourseManagementWebConfiguration>(););
    }
}
