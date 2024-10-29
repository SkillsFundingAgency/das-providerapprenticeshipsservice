namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

public interface IAuthorizationHandler
{
    string Prefix { get; }
    Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext);
}