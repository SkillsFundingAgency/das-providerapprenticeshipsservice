namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public interface IActionContextAccessorWrapper
{
    RouteData GetRouteData();
};

public class ActionContextAccessorWrapper : IActionContextAccessorWrapper
{
    private readonly IActionContextAccessor _actionContextAccessor;
    public ActionContextAccessorWrapper(IActionContextAccessor actionContextAccessor)
    {
        _actionContextAccessor = actionContextAccessor;
    }

    public RouteData GetRouteData()
    {
        return _actionContextAccessor.ActionContext.RouteData;
    }
}