namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

/// <summary>
/// Default Handler been introduced not to break the existing usage of this Package.
/// Actual implementation been done in the calling project to give the flexibility of the usage of this handler.
/// </summary>
public class DefaultAuthorizationHandler : IDefaultAuthorizationHandler
{
    public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
    {
        return Task.FromResult(new AuthorizationResult());
    }      
}