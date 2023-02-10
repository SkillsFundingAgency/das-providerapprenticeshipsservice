using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations
{
    public static class OrchestratorsServiceRegistrations
    {
        public static IServiceCollection AddOrchestrators(this IServiceCollection services)
        {
            services.AddTransient<IAccountOrchestrator, AccountOrchestrator>();
            services.AddTransient<IAgreementOrchestrator, AgreementOrchestrator>();
            services.AddTransient<IAuthenticationOrchestrator, AuthenticationOrchestrator>();

            return services;
        }
    }
}
