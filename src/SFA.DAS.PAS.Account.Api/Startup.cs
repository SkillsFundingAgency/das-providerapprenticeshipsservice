using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SFA.DAS.PAS.Account.Api.Startup))]

namespace SFA.DAS.PAS.Account.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
