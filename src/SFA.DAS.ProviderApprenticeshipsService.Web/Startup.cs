using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.WsFederation;
using NLog;
using Owin;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
          // BuildConfiguration(app);
        }

        // private void BuildConfiguration(IAppBuilder app)
        // { 
        //     var configuration = app.
        //
        //     var configBuilder = new ConfigurationBuilder()
        //         .AddConfiguration(configuration);
        //
        //     throw new NotImplementedException();
        // }
    }
}
