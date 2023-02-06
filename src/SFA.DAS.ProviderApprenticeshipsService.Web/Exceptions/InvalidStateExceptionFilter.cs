using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions
{
    public class InvalidStateExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(InvalidStateException))
            {
                LogManager.GetCurrentClassLogger().Info(filterContext.Exception, "Invalid state exception");

                filterContext.ExceptionHandled = true;

                //var rvd = new RouteValueDictionary {{"message", "Cannot access the requested page"}};
                //filterContext.Result = new RedirectToRouteResult("home", rvd);
            }
        }
    }
}
