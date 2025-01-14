using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

public static class OrchestratorsServiceRegistrations
{
    public static void AddOrchestrators(this IServiceCollection services)
    {
        services.AddTransient<IAccountOrchestrator, AccountOrchestrator>();
        services.AddTransient<IAuthenticationOrchestrator, AuthenticationOrchestrator>();
    }
}