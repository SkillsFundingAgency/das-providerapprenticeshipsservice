using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Logging;

public class AuthorizationResultLogger(IAuthorizationHandler authorizationHandler, ILogger<AuthorizationResultLogger> logger)
    : IAuthorizationHandler
{
    public string Prefix => authorizationHandler.Prefix;

    public async Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
    {
        var authorizationResult = await authorizationHandler.GetAuthorizationResult(options, authorizationContext).ConfigureAwait(false);           
            
        authorizationContext.TryGet("AccountId", out long? accountId);
        accountId = accountId.HasValue ? accountId : 0;
        authorizationContext.TryGet("HashedAccountId", out string hashedAccountId);
        authorizationContext.TryGet("UserRef", out Guid userRef);
        var message = $"Finished running handler with prefix '{Prefix}' for options '{string.Join(", ", options)}' and context  AccountId: '{accountId }' HashedAccountId: '{hashedAccountId}' UserRef: '{userRef}'  with result '{authorizationResult}'";

        if (authorizationResult.IsAuthorized)
        {
            logger.LogInformation(message);
        }
        else
        {
            logger.LogWarning(message);
        }

        return authorizationResult;
    }
}