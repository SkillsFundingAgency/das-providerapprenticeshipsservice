namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public static class AuthorizationContextKey
{
    public const string AccountLegalEntityId = "AccountLegalEntityId";
    public const string Ukprn = "Ukprn";
    public const string UserEmail = "UserEmail";
}

public static class AuthorizationContextExtensions
{
    public static void AddProviderPermissionValues(this IAuthorizationContext authorizationContext, long accountLegalEntityId, long ukprn)
    {
        authorizationContext.Set(AuthorizationContextKey.Ukprn, ukprn);
        authorizationContext.Set(AuthorizationContextKey.AccountLegalEntityId, accountLegalEntityId);
    }
    
    public static void AddProviderFeatureValues(this IAuthorizationContext authorizationContext, long ukprn, string userEmail)
    {
        authorizationContext.Set(AuthorizationContextKey.Ukprn, ukprn);
        authorizationContext.Set(AuthorizationContextKey.UserEmail, userEmail);
    }
    
    public static (long Ukprn, string UserEmail) GetProviderFeatureValues(this IAuthorizationContext authorizationContext)
    {
        return (authorizationContext.Get<long>(AuthorizationContextKeys.Ukprn),
            authorizationContext.Get<string>(AuthorizationContextKeys.UserEmail));
    }
}