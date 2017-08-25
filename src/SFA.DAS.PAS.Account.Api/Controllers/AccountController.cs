using System.Threading.Tasks;
using System.Web.Http;

using SFA.DAS.PAS.Account.Api.Orchestrator;

namespace SFA.DAS.PAS.Account.Api.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private readonly AccountOrchestrator _orchestrator;

        public AccountController(AccountOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("{ukprn}/users")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccountUsers(long ukprn)
        {
            var result = await _orchestrator.GetAccountUsers(ukprn);

            return Ok(result);
        }
    }
}