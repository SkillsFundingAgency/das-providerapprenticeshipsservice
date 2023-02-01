﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations
{
    public static class ConfigurationServiceRegistrations
    {
        public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IProviderAgreementStatusConfiguration>(configuration.Get<ProviderApprenticeshipsServiceConfiguration>());

            return services;
        }
    }
}
