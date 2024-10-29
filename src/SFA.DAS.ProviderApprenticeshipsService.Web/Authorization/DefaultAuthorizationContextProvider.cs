namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public interface IAuthorizationContextProvider
{
    IAuthorizationContext GetAuthorizationContext();
}

public class DefaultAuthorizationContextProvider : IAuthorizationContextProvider
{
    public IAuthorizationContext GetAuthorizationContext()
    {
        return new AuthorizationContext();
    }
}