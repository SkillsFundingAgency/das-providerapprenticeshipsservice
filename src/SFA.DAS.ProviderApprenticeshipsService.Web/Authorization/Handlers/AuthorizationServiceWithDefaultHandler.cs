namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

public class AuthorizationServiceWithDefaultHandler(
    IAuthorizationContextProvider authorizationContextProvider,
    IDefaultAuthorizationHandler defaultAuthorizationHandler,
    IAuthorizationService authorizationService)
    : IAuthorizationService
{
    //
    // public void Authorize(params string[] options)
    // {
    //     _authorizationService.Authorize(options);
    // }
    //
    // public async Task AuthorizeAsync(params string[] options)
    // {
    //     await _authorizationService.AuthorizeAsync(options);
    // }
    //
    // public virtual AuthorizationResult GetAuthorizationResult(params string[] options)
    // {
    //     return _authorizationService.GetAuthorizationResult(options);
    // }

    public async Task<AuthorizationResult> GetAuthorizationResultAsync(params string[] options)
    {
        var authorizationTask = authorizationService.GetAuthorizationResultAsync(options);

        var defaultAuthorizationTask = defaultAuthorizationHandler.GetAuthorizationResult(options, authorizationContextProvider.GetAuthorizationContext());

        await Task.WhenAll(authorizationTask, defaultAuthorizationTask);

        var authorizationResult = authorizationTask.Result;
        var defaultAuthorizationResult = defaultAuthorizationTask.Result;

        if (defaultAuthorizationResult != null)
        {
            foreach (var err in defaultAuthorizationResult.Errors)
            {
                authorizationResult.AddError(err);
            }
        }

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