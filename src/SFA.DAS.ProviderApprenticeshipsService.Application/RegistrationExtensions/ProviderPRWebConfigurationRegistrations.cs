using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

public static class ProviderPRWebConfigurationRegistrations
{
    public static IServiceCollection AddProviderPRWebConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ProviderPRWebConfiguration>(configuration.GetSection(nameof(ProviderPRWebConfiguration)));
        services.AddSingleton<IProviderPRWebConfiguration>(sp => sp.GetRequiredService<IOptions<ProviderPRWebConfiguration>>().Value);

        return services;
    }
}
