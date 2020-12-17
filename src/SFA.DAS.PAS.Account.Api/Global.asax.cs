using System.Configuration;
using Microsoft.ApplicationInsights.Extensibility;
using System.Web.Http;

namespace SFA.DAS.PAS.Account.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // LoggingConfig.ConfigureLogging();

            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
