using System.Threading.Tasks;
using System.Web.Http;

using SFA.DAS.PAS.Account.Api.Orchestrator;

namespace SFA.DAS.PAS.Account.Api.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly UserOrchestrator _orchestrator;

        public UserController(UserOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("{userRef}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserSettings(string userRef)
        {
            var result = await _orchestrator.GetUser(userRef);

            return Ok(result);
        }
    }
}