using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using System.Web.Http;

namespace SFA.DAS.PAS.Account.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // LoggingConfig.ConfigureLogging();

            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("InstrumentationKey");

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
