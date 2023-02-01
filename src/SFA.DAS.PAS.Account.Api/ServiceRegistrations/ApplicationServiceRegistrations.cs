using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations
{
    public static class ApplicationServiceRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IBackgroundNotificationService, BackgroundNotificationService>();
            services.AddTransient<IProviderCommitmentsLogger, ProviderCommitmentsLogger>();

            services.AddTransient<ICurrentDateTime, CurrentDateTime>();

            return services;
        }
    }
}
