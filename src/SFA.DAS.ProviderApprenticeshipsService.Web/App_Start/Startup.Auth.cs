using System;
using System.Configuration;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Owin;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public partial class Startup
    {
        private static readonly string Realm = ConfigurationManager.AppSettings["ida:Wtrealm"];
        private static readonly string AdfsMetadata = ConfigurationManager.AppSettings["ida:ADFSMetadata"];

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            //todo: may need more options here
            var options = new WsFederationAuthenticationOptions
            {
                Wtrealm = Realm,
                MetadataAddress = AdfsMetadata
                //,Wreply = "?"
                //,SignOutWreply = "?"
            };

            app.UseWsFederationAuthentication(options);
        }
    }
}
