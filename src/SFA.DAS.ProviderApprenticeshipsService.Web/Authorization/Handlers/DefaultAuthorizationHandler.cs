namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

public class DefaultAuthorizationHandler : IDefaultAuthorizationHandler
{
    public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
    {
        return Task.FromResult(new AuthorizationResult());
    }      
}