using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

public static class ApprenticeInfoServiceRegistrations
{
    public static IServiceCollection AddApprenticeshipInfoService(this IServiceCollection services)
    {
        services.AddTransient<IApprenticeshipInfoService, ApprenticeshipInfoService>();

        return services;
    }
}