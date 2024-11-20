namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public interface IAuthorizationContextProvider
{
    IAuthorizationContext GetAuthorizationContext();
}
