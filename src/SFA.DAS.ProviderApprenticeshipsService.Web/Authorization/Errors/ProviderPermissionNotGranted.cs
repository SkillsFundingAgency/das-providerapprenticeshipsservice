namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Errors;

public class ProviderPermissionNotGranted : AuthorizationError
{
    public ProviderPermissionNotGranted() : base("Provider permission is not granted")
    {
    }
}