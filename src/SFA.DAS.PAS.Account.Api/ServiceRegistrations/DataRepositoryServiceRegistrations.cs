using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations;

public static class DataRepositoryServiceRegistrations
{
    public static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserSettingsRepository, UserSettingsRepository>();
        services.AddTransient<IProviderAgreementStatusRepository, ProviderAgreementStatusRepository>();
        services.AddTransient<IUserRepository, UserRepository>();

        return services;
    }
}