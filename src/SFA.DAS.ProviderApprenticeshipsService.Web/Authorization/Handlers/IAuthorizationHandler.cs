namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

public interface IDefaultAuthorizationHandler
{
    Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext);
}

public interface IAuthorizationHandler : IDefaultAuthorizationHandler
{
    string Prefix { get; }
}