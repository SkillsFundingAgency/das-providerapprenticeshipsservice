using System;
using System.Net;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentValidation.Mvc;
using NLog;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
            FluentValidationModelValidatorProvider.Configure();

            Logger.Info("Starting up");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError().GetBaseException();
            Logger.Error(ex, "Unhanded Exception");

            if (ex.GetType() == typeof(ArgumentException))
            {
                Server.ClearError();
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }
}
