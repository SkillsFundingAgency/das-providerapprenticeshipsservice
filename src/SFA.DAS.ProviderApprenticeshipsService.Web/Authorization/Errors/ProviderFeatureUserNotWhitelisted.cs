namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Errors;

public class ProviderFeatureUserNotWhitelisted : AuthorizationError
{
    public ProviderFeatureUserNotWhitelisted() : base("Provider feature user not whitelisted")
    {
    }
}