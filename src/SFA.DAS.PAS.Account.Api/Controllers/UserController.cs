using System.Threading.Tasks;
using System.Web.Http;

using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly UserOrchestrator _orchestrator;

        private readonly IProviderCommitmentsLogger _logger;

        public UserController(UserOrchestrator orchestrator, IProviderCommitmentsLogger logger)
        {
            _orchestrator = orchestrator;
            _logger = logger;
        }

        [Route("{userRef}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserSettings(string userRef)
        {
            _logger.Info($"Getting users settings for user: {userRef}");
            var result = await _orchestrator.GetUser(userRef);
            _logger.Info($"Found {(result == null ? "0" : "1")}  users settings for user: {userRef}");

            return Ok(result);
        }
    }
}