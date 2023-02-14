namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    public static class PolicyNames
    {
        public static string RequireAuthenticatedUser => nameof(RequireAuthenticatedUser);
        public static string RequireDasPermissionRole => nameof(RequireDasPermissionRole);
    }
}
