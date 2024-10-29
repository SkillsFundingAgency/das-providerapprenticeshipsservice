using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Services;

public interface IAuthorizationService
{
    void Authorize(params string[] options);
    Task AuthorizeAsync(params string[] options);
    AuthorizationResult GetAuthorizationResult(params string[] options);
    Task<AuthorizationResult> GetAuthorizationResultAsync(params string[] options);
    bool IsAuthorized(params string[] options);
    Task<bool> IsAuthorizedAsync(params string[] options);
}

public class AuthorizationService(IAuthorizationContextProvider authorizationContextProvider, IEnumerable<IAuthorizationHandler> handlers)
    : IAuthorizationService
{
    public void Authorize(params string[] options)
    {
        AuthorizeAsync(options).GetAwaiter().GetResult();
    }

    public async Task AuthorizeAsync(params string[] options)
    {
        var isAuthorized = await IsAuthorizedAsync(options).ConfigureAwait(false);

        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException();
        }
    }

    public AuthorizationResult GetAuthorizationResult(params string[] options)
    {
        return GetAuthorizationResultAsync(options).GetAwaiter().GetResult();
    }

    public async Task<AuthorizationResult> GetAuthorizationResultAsync(params string[] options)
    {
        var authorizationContext = authorizationContextProvider.GetAuthorizationContext();
        var unrecognizedOptions = options.Where(o => !handlers.Any(h => o.StartsWith(h.Prefix))).ToList();

        if (unrecognizedOptions.Count > 0)
        {
            throw new ArgumentException($"Options '{string.Join(", ", unrecognizedOptions)}' were unrecognized", nameof(options));
        }
            
        var authorizationResults = await Task.WhenAll(
            from h in handlers
            let o = options.Where(o => o.StartsWith(h.Prefix)).Select(o => o.Replace(h.Prefix, "")).ToList()
            select h.GetAuthorizationResult(o, authorizationContext)).ConfigureAwait(false);
            
        var authorizationResult = new AuthorizationResult(authorizationResults.SelectMany(r => r.Errors));
            
        return authorizationResult;
    }

    public bool IsAuthorized(params string[] options)
    {
        return IsAuthorizedAsync(options).GetAwaiter().GetResult();
    }

    public async Task<bool> IsAuthorizedAsync(params string[] options)
    {
        var authorizationResult = await GetAuthorizationResultAsync(options).ConfigureAwait(false);

        return authorizationResult.IsAuthorized;
    }
}