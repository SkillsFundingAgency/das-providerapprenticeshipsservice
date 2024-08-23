using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

public static class DataRepositoryServiceRegistrations
{
    public static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserSettingsRepository, UserSettingsRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IProviderAgreementStatusRepository, ProviderAgreementStatusRepository>();
        services.AddTransient<IProviderRepository, ProviderRepository>();

        services.AddSingleton(new ChainedTokenCredential(
            new ManagedIdentityCredential(),
            new AzureCliCredential())
        );

        return services;
    }
}