using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.LinkGeneratorService;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

public static class LinkGeneratorServiceRegistrations
{
    public static IServiceCollection AddLinkGenerator(this IServiceCollection services)
    {
        services.AddTransient<IEnvironmentService, EnvironmentService>();
        services.AddTransient<IAzureTableStorageConnectionAdapter, AzureTableStorageConnectionAdapter>();
        services.AddTransient<IAutoConfigurationService, TableStorageConfigurationService>();
        services.AddTransient<ILinkGeneratorService, LinkGeneratorService>();

        return services;
    }
}