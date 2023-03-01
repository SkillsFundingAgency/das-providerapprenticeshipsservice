using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.UserIdentityService;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class UserIdentityServiceRegistrations
    {
        public static IServiceCollection AddUserIdentityService(this IServiceCollection services)
        {
            services.AddTransient<IUserIdentityService, UserIdentityService>();

            return services;
        }
    }
}
