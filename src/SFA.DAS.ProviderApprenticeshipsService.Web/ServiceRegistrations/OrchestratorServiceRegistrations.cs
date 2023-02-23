﻿using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations
{
    public static class OrchestratorsServiceRegistrations
    {
        public static IServiceCollection AddOrchestrators(this IServiceCollection services)
        {
            services.AddTransient<IAgreementMapper, AgreementMapper>();
            services.AddTransient<IAccountOrchestrator, AccountOrchestrator>();
            services.AddTransient<IAgreementOrchestrator, AgreementOrchestrator>();
            services.AddTransient<IAuthenticationOrchestrator, AuthenticationOrchestrator>();

            return services;
        }
    }
}