using System;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
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
            var urlReferrer = filterContext.HttpContext.Request.GetDisplayUrl();
            var referrer = urlReferrer == null ? "unknown" : urlReferrer.ToString();

            var rawUrl = filterContext.HttpContext.Request.GetUri();

            var controller = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ControllerName;
            var actionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ActionName;

            Logger.Info($"To track Apprentice V1 details UrlReferrer Request: {referrer} Request to Page: {rawUrl} Handled At: {controller}.{actionName}");
            base.OnActionExecuting(filterContext);
        }
    }
}