namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;

public static class RoleNames
{
    public static string DasPermission => nameof(DasPermission);

    public static string HasViewerOrAbovePermission => nameof(HasViewerOrAbovePermission);

    public static string HasContributorOrAbovePermission => nameof(HasContributorOrAbovePermission);

    public static string HasContributorWithApprovalOrAbovePermission => nameof(HasContributorWithApprovalOrAbovePermission);

    public static string HasAccountOwnerPermission => nameof(HasAccountOwnerPermission);
}