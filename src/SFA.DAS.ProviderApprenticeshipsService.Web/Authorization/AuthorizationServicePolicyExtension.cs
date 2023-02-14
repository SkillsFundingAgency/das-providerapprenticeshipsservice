using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.ProviderPermissions.Handlers;
using SFA.DAS.Authorization.Caching;
using SFA.DAS.Authorization.Logging;
using SFA.DAS.Authorization.Services;
using StructureMap.Pipeline;
using StructureMap;
using SFA.DAS.ProviderRelationships.Api.Client.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.Features.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    public static class AuthorizationServicePolicyExtension
    {
        public static void AddAuthorizationServicePolicies(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PolicyNames
                        .RequireAuthenticatedUser
                    , policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });

                options.AddPolicy(
                    PolicyNames
                        .RequireDasPermissionRole
                    , policy =>
                    {
                        policy.RequireRole(RoleNames.DasPermission);
                    });
            });

            // ***Deafult Registry***
            For<IAuthorizationContextProvider>().Use<AuthorizationContextProvider>();
            For<IAuthorizationHandler>().Use<AuthorizationHandler>();
            // replacing the above with:
            services.AddAuthorization<AuthorizationContextProvider>();
            services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();

            // ***AuthorisationRegistry***
            For<IAuthorizationContext>().Use((IContext c) => c.GetInstance<IAuthorizationContextProvider>().GetAuthorizationContext());
            For<IAuthorizationContextProvider>().Use<DefaultAuthorizationContextProvider>();
            For<IAuthorizationContextProvider>().DecorateAllWith<AuthorizationContextCache>();
            For<IAuthorizationHandler>().DecorateAllWith<AuthorizationResultLogger>();
            SmartInstance<AuthorizationService, IAuthorizationService> instance = For<IAuthorizationService>().Use<AuthorizationService>();
            For<IDefaultAuthorizationHandler>().Use<DefaultAuthorizationHandler>();
            For<IAuthorizationService>().Use<AuthorizationServiceWithDefaultHandler>().Ctor<IAuthorizationService>().Is(instance);
            ForConcreteType<object>().Configure.Named("AuthorizationRegistry");
            // replacing the above with:
            services.AddSingleton<IAuthorizationContext>(_ => _.GetService<IAuthorizationContextProvider>().GetAuthorizationContext());
            services.AddSingleton<IAuthorizationContextProvider, DefaultAuthorizationContextProvider>();
            services.Decorate<IAuthorizationContextProvider, AuthorizationContextCache>();
            services.Decorate<IAuthorizationHandler, AuthorizationResultLogger>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddSingleton<IDefaultAuthorizationHandler, DefaultAuthorizationHandler>();
            services.AddScoped<IAuthorizationService, AuthorizationServiceWithDefaultHandler>();

            // ***ProviderPermissionsAuthorisationRegistry***
            For<IAuthorizationHandler>().Add< SFA.DAS.Authorization.ProviderPermissions.Handlers.AuthorizationHandler >(); // this is a dupe
            IncludeRegistry<ProviderRelationshipsApiClientRegistry>(); // I think this is replaced by whats been done in ProviderRelationshipsApiClientRegistrations

            // ***ProviderFeaturesAuthorizationRegistry***
            For<IAuthorizationHandler>().Add< SFA.DAS.Authorization.ProviderFeatures.Handlers.AuthorizationHandler >(); // here the handler comes from somewhere else,not sure if needed
            For<IFeatureTogglesService< SFA.DAS.Authorization.ProviderFeatures.Models.ProviderFeatureToggle >>().Use<FeatureTogglesService<
                SFA.DAS.Authorization.ProviderFeatures.ConfigurationProviderFeaturesConfiguration, SFA.DAS.Authorization.ProviderFeatures.Models.ProviderFeatureToggle>>().Singleton(); // this has been done in FeatureToggleServiceRegistrations
        }
    }
}
