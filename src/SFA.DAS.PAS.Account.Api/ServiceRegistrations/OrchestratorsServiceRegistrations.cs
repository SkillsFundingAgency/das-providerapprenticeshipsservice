using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PAS.Account.Api.Orchestrator;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations
{
    public static class OrchestratorsServiceRegistrations
    {
        public static IServiceCollection AddOrchestrators(this IServiceCollection services)
        {
            services.AddTransient<IAccountOrchestrator, AccountOrchestrator>();
            services.AddTransient<IEmailOrchestrator, EmailOrchestrator>();
            services.AddTransient<IUserOrchestrator, UserOrchestrator>();

            return services;
        }
    }
}
