using System;
using System.Web.Mvc;
using System.Web.Routing;
using NLog;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions
{
    public class InvalidStateExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(InvalidStateException))
            {
                LogManager.GetCurrentClassLogger().Error(filterContext.Exception, "Invalid state exception");

                var rvd = new RouteValueDictionary {{"message", "Cannot access the requested page"}}; //todo: revise message

                filterContext.ExceptionHandled = true;
                filterContext.Result = new RedirectToRouteResult("home", rvd);
            }
        }
    }
}
