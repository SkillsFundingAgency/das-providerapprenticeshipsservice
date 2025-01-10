namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public abstract class AuthorizationError(string message)
{
    public string Message { get; } = message;
}