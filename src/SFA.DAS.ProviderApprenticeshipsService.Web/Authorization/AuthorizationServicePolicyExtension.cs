using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    public static class AuthorizationServicePolicyExtension
    {
        public static void AddAuthorizationServicePolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PolicyNames
                        .RequireAuthenticatedUser
                    , policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });
            });
        }
    }
}
