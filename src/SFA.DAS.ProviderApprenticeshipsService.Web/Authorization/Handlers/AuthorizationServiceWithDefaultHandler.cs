using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

public class AuthorizationServiceWithDefaultHandler(
    IAuthorizationContextProvider authorizationContextProvider,
    IDefaultAuthorizationHandler defaultAuthorizationHandler,
    IAuthorizationService authorizationService)
    : IAuthorizationService
{
    public async Task<AuthorizationResult> GetAuthorizationResultAsync(params string[] options)
    {
        var authorizationTask = authorizationService.GetAuthorizationResultAsync(options);

        var defaultAuthorizationTask = defaultAuthorizationHandler.GetAuthorizationResult(options, authorizationContextProvider.GetAuthorizationContext());

        await Task.WhenAll(authorizationTask, defaultAuthorizationTask);

        var authorizationResult = authorizationTask.Result;
        var defaultAuthorizationResult = defaultAuthorizationTask.Result;

        if (defaultAuthorizationResult == null)
        {
            return authorizationResult;
        }

        foreach (var err in defaultAuthorizationResult.Errors)
        {
            authorizationResult.AddError(err);
        }

        return authorizationResult;
    }

    public async Task<bool> IsAuthorizedAsync(params string[] options)
    {
        var authorizationResult = await GetAuthorizationResultAsync(options).ConfigureAwait(false);

        return authorizationResult.IsAuthorized;
    }
}