using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;

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
        }
    }
}
