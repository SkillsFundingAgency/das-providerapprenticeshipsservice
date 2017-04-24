using System;
using System.Web.Mvc;
using NLog;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions
{
    public class InvalidStateExceptionFilter : FilterAttribute, IExceptionFilter
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
