namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public interface IActionContextAccessorWrapper
{
    RouteData GetRouteData();
};

public class ActionContextAccessorWrapper(IActionContextAccessor actionContextAccessor) : IActionContextAccessorWrapper
{
    public RouteData GetRouteData()
    {
        return actionContextAccessor.ActionContext.RouteData;
    }
}