using System.Security.Principal;
using System.Web;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests
{
    public class FakeHttpContext : HttpContextBase
    {
        public override IPrincipal User { get; set; }
    }
}