using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class DataRepositoryServiceRegistrations
    {
        public static IServiceCollection AddDataRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserSettingsRepository, UserSettingsRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}
