using System;
using Owin;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
