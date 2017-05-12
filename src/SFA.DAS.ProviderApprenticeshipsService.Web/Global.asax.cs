﻿using System;
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

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;

            FluentValidationModelValidatorProvider.Configure();

            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("InstrumentationKey");

            Logger.Info("Starting up");
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            application?.Context?.Response.Headers.Remove("Server");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError().GetBaseException();

            if (ex is HttpException)
            {
                Logger.Warn(ex, "Http Exception");
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
    }
}
