﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations
{
    public static class ApplicationServiceRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfigurations(configuration);
            services.AddDataRepositories();
            services.AddCommitments(configuration);
            services.AddContentApi(configuration);
            services.AddNotifications(configuration);
            services.AddRoatpServices(configuration);
            services.AddProviderRelationshipsApi(configuration);
            services.AddFluentValidation();
            services.AddApprenticeshipInfoService();
                     
            services.AddHashingServices(configuration);
            services.AddCookieStorageService();
            services.AddLinkGenerator();
            services.AddMediatRHandlers();

            // the below is closely tied to Web, so not sure where logically best to register them
            services.AddTransient<IProviderCommitmentsLogger, ProviderCommitmentsLogger>(); // need to think where to inject it > move to Application
            services.AddScoped<IHtmlHelpers, HtmlHelpers>(); // to be grouped to somewhere else
            services.AddScoped<IAuthenticationServiceWrapper, AuthenticationServiceWrapper>(); // this is unused atm, to be confirmed if this is a preferred way in AccountController
            

            return services;
        }
    } 
}
