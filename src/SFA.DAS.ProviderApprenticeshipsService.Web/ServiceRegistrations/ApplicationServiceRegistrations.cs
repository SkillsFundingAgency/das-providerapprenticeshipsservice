using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations
{
    public static class ApplicationServiceRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfigurations(configuration);
            services.AddAccountApiClient(configuration);
            services.AddDataRepositories();
            services.AddCommitmentsV2ApiClient(configuration);
            services.AddContentApi(configuration);
            services.AddNotifications(configuration);
            services.AddProviderRelationshipsApi(configuration); // TBC IF NEEDED
            services.AddFluentValidation();
            services.AddApprenticeshipInfoService();
            services.AddUserIdentityService();

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
