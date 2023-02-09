using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using Microsoft.Extensions.Options;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations
{
    public static class RoatpCourseManagementWebServiceRegistrations
    {
        public static IServiceCollection AddRoatpCourseManagementWebConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRoatpCourseManagementWebConfiguration>(configuration.Get<RoatpCourseManagementWebConfiguration>());
            // services.AddSingleton<IRoatpCourseManagementWebConfiguration>(cfg => cfg.GetService<IOptions<RoatpCourseManagementWebConfiguration>>().Value);

            return services;
        }
        // REPLCAING:
        // For<IRoatpCourseManagementWebConfiguration>().Use(configurationService.Get<RoatpCourseManagementWebConfiguration>(););
    }
}
