using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations
{
    public static class ApplicationServiceRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCommitments(configuration);
            services.AddContentApi(configuration);
            services.AddNotifications(configuration);
            services.AddRoatpServices(configuration);
            services.AddProviderRelationshipsApi(configuration);
            services.AddMediatRHandlers();

            return services;
        }
    } 
}
