using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public interface IAuthorizationService
{
    Task<bool> IsAuthorizedAsync(params string[] options);
    Task<AuthorizationResult> GetAuthorizationResultAsync(params string[] options);
}

public class AuthorizationService : IAuthorizationService
{
    private readonly IAuthorizationContextProvider _authorizationContextProvider;
    private readonly IEnumerable<IAuthorizationHandler> _handlers;

    public AuthorizationService(IAuthorizationContextProvider authorizationContextProvider, IEnumerable<IAuthorizationHandler> handlers)
    {
        _authorizationContextProvider = authorizationContextProvider;
        _handlers = handlers;
    }
    
    public async Task<AuthorizationResult> GetAuthorizationResultAsync(params string[] options)
    {
        var authorizationContext = _authorizationContextProvider.GetAuthorizationContext();
        var unrecognizedOptions = options.Where(o => !_handlers.Any(h => o.StartsWith(h.Prefix))).ToList();

        if (unrecognizedOptions.Count > 0)
        {
            throw new ArgumentException($"Options '{string.Join(", ", unrecognizedOptions)}' were unrecognized", nameof(options));
        }

        var authorizationResults = await Task.WhenAll(
            from h in _handlers
            let o = options.Where(o => o.StartsWith(h.Prefix)).Select(o => o.Replace(h.Prefix, "")).ToList()
            select h.GetAuthorizationResult(o, authorizationContext)).ConfigureAwait(false);

        var authorizationResult = new AuthorizationResult(authorizationResults.SelectMany(r => r.Errors));

        return authorizationResult;
    }
    
    

    public async Task<bool> IsAuthorizedAsync(params string[] options)
    {
        var authorizationResult = await GetAuthorizationResultAsync(options).ConfigureAwait(false);

        return authorizationResult.IsAuthorized;
    }
}