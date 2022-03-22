using System.Configuration;
using Microsoft.Owin.Security.ActiveDirectory;

using Owin;

namespace SFA.DAS.PAS.Account.Api
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
               new WindowsAzureActiveDirectoryBearerAuthenticationOptions
               {
                   TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                   {
                       RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                       ValidAudiences = ConfigurationManager.AppSettings["idaAudience"].Split(',')
                   },
                   Tenant = ConfigurationManager.AppSettings["idaTenant"]
               });
        }
    }
}
