using Microsoft.Extensions.Configuration;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;

public static class AuthenticationExtensions
{
    public static void AddAndConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAndConfigureDfESignInAuthentication(
            configuration,
            "SFA.DAS.ProviderApprenticeshipService",
            typeof(CustomServiceRole),
            ClientName.ProviderRoatp,
            "/signout",
            "");
        
    }
}