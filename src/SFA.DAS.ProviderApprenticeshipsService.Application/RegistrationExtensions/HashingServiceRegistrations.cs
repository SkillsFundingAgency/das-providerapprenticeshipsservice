using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class HashingServiceRegistrations
    {
        public static IServiceCollection AddHashingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // REPLACING: 
            //For<IHashingService>().Use(x => new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring));
            //For<IPublicHashingService>().Use(x => new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring)); // UNUSED: TO BE CONFIRMD IF CAN BE REMOVED
            //For<IAccountLegalEntityPublicHashingService>().Use(x => new PublicHashingService(config.PublicAllowedAccountLegalEntityHashstringCharacters, config.PublicAllowedAccountLegalEntityHashstringSalt));

            services.AddTransient<IHashingService>(_ =>
            {
                var config = _.GetService<ProviderApprenticeshipsServiceConfiguration>();
                return new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring);
            });

            services.AddTransient<IPublicHashingService>(_ =>
            {
                var config = _.GetService<ProviderApprenticeshipsServiceConfiguration>();
                return new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring);
            });

            services.AddTransient<IAccountLegalEntityPublicHashingService>(_ =>
            {
                var config = _.GetService<ProviderApprenticeshipsServiceConfiguration>();
                return new PublicHashingService(config.PublicAllowedAccountLegalEntityHashstringCharacters, config.PublicAllowedAccountLegalEntityHashstringSalt);
            });

            return services;
        }
    }
}
