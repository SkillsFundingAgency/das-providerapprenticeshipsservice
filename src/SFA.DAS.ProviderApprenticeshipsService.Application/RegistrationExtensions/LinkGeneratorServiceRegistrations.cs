using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.LinkGeneratorService;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class LinkGeneratorServiceRegistrations
    {
        public static IServiceCollection AddLinkGenerator(this IServiceCollection services)
        {
            services.AddSingleton<ILinkGeneratorService, LinkGeneratorService>();

            return services;
        }
    }
}
