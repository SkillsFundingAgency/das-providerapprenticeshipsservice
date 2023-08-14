namespace SFA.DAS.PAS.Account.Api.Authorization;

public static class AuthorizationExtensions
{
    private static readonly string[] PolicyNames = {
        ApiRoles.ReadUserSettings,
        ApiRoles.ReadAccountUsers,
    };

    private const string DefaultPolicyName = "default";

    public static IServiceCollection AddApiAuthorization(this IServiceCollection services, bool isDevelopment = false)
    {
        services.AddAuthorization(options =>
        {
            AddDefaultPolicy(isDevelopment, options);

            AddPolicies(isDevelopment, options);
            
            options.DefaultPolicy = options.GetPolicy(DefaultPolicyName);
        });

        if (isDevelopment)
        {
            services.AddSingleton<IAuthorizationHandler, LocalAuthorizationHandler>();
        }

        return services;
    }

    private static void AddPolicies(bool isDevelopment, AuthorizationOptions options)
    {
        foreach (var policyName in PolicyNames)
        {
            options.AddPolicy(policyName, policy =>
            {
                if (isDevelopment)
                    policy.AllowAnonymousUser();
                else
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(policyName);
                }
            });
        }
    }

    private static void AddDefaultPolicy(bool isDevelopment, AuthorizationOptions options)
    {
        options.AddPolicy(DefaultPolicyName, policy =>
        {
            if (isDevelopment)
                policy.AllowAnonymousUser();
            else
                policy.RequireAuthenticatedUser();
        });
    }
}