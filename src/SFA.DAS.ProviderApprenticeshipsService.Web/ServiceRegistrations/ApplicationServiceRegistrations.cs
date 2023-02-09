using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations
{
    public static class ApplicationServiceRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEncodingServices(configuration);
            services.AddNotifications(configuration);
            services.AddRoatpCourseManagementWebConfiguration(configuration);

            return services;
        }
    } 
}
