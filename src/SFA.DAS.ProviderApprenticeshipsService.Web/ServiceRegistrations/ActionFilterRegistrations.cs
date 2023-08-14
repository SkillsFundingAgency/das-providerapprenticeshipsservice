using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

public static class ActionFilterRegistrations
{
    public static void AddActionFilters(this IServiceCollection services)
    {
        services.AddScoped<ProviderUkPrnCheckActionFilter>();
    }
}