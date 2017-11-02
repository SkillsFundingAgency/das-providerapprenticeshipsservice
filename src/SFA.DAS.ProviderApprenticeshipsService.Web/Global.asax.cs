using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentValidation.Mvc;
using SFA.DAS.NLog.Logger;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using SFA.DAS.ProviderApprenticeshipsService.Web.App_Start;
using System.Linq;
using System.Net;
using SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution;
using SFA.DAS.Web.Policy;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public class MvcApplication : HttpApplication
    {
        private static ILog Logger = new NLogLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(string), new TrimStringModelBinder());

            AntiForgeryConfig.UniqueClaimTypeIdentifier = DasClaimTypes.Name;

            var container = StructuremapMvc.StructureMapDependencyScope.Container;
            FluentValidationModelValidatorProvider.Configure(x => x.ValidatorFactory = new StructureMapValidatorFactory(container));

            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("InstrumentationKey");

            Logger.Info("Starting up");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError().GetBaseException();
            var httpEx = ex as HttpException;

            if (httpEx != null && httpEx.GetHttpCode() == (int)HttpStatusCode.Forbidden)
            {
                Logger.Info($"{ex.Message} ({HttpStatusCode.Forbidden})");
            }
            if (httpEx != null && httpEx.GetHttpCode() == (int)HttpStatusCode.NotFound)
            {
                Logger.Warn($"NotFound (404): {Request.HttpMethod} {Request.Url}");
            }
            else if (httpEx != null)
            {
                Logger.Warn(ex, $"Http Exception {httpEx.GetHttpCode()}");
            }
            else if (ex is InvalidOperationException && ex.Message.StartsWith("A claim of type"))
            {
                var claims = ((ClaimsIdentity)Context.User.Identity).Claims
                        .Select(x => $"{x.Type}: {x.Value}").ToArray();

                var logValue = string.Join(Environment.NewLine, claims);

                Logger.Error(ex, $"Invalid Claims: {Environment.NewLine}{logValue}{Environment.NewLine}");
            }
            else
            {
                Logger.Error(ex, "Unhandled Exception");
            }
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            new HttpContextPolicyProvider(
                new List<IHttpContextPolicy>()
                {
                    new ResponseHeaderRestrictionPolicy()
                }
            ).Apply(new HttpContextWrapper(HttpContext.Current), PolicyConcern.HttpResponse);
        }
    }
}
