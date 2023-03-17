using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.PAS.Account.Api.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddApiAuthorization(this IServiceCollection services, bool isDevelopment = false)
        {
            services.AddAuthorization(x =>
            {
                {
                    x.AddPolicy("default", policy =>
                    {
                        if (isDevelopment)
                            policy.AllowAnonymousUser();
                        else
                            policy.RequireAuthenticatedUser();

                    });

                    x.AddPolicy(ApiRoles.ReadUserSettings, policy =>
                    {
                        if (isDevelopment)
                            policy.AllowAnonymousUser();
                        else
                        {
                            policy.RequireAuthenticatedUser();
                            policy.RequireRole(ApiRoles.ReadUserSettings);
                        }
                    });

                    x.AddPolicy(ApiRoles.ReadAccountUsers, policy =>
                    {
                        if (isDevelopment)
                            policy.AllowAnonymousUser();
                        else
                        {
                            policy.RequireAuthenticatedUser();
                            policy.RequireRole(ApiRoles.ReadAccountUsers);
                        }
                    });

                    x.DefaultPolicy = x.GetPolicy("default");
                }
            });
            if (isDevelopment)
                services.AddSingleton<IAuthorizationHandler, LocalAuthorizationHandler>();

            return services;
        }
    }
}