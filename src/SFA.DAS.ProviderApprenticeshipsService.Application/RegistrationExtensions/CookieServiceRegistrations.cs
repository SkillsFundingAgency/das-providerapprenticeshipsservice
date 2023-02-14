using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class CookieServiceRegistrations
    {
        public static IServiceCollection AddCookieStorageService(this IServiceCollection services)
        {
            services.AddScoped(typeof(ICookieService<>), typeof(HttpCookieService<>));
            services.AddScoped(typeof(ICookieStorageService<>), typeof(CookieStorageService<>));

            return services;
        }
    }
}
