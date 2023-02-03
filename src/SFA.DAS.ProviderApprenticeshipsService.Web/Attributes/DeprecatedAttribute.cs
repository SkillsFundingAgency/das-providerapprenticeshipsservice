using System;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DeprecatedAttribute : ActionFilterAttribute
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var urlReferrer = filterContext.HttpContext.Request.UrlReferrer;
            var referrer = urlReferrer == null ? "unknown" : urlReferrer.ToString();

            var rawUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;

            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = filterContext.ActionDescriptor.ActionName;

            Logger.Info($"To track Apprentice V1 details UrlReferrer Request: {referrer} Request to Page: {rawUrl} Handled At: {controllerName}.{actionName}");
            base.OnActionExecuting(filterContext);
        }
    }
}