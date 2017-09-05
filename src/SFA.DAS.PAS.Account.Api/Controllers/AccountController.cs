using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private readonly AccountOrchestrator _orchestrator;

        private readonly IProviderCommitmentsLogger _logger;

        public AccountController(AccountOrchestrator orchestrator, IProviderCommitmentsLogger logger)
        {
            _orchestrator = orchestrator;
            _logger = logger;
        }

        [Route("{ukprn}/users")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAccountUsers(long ukprn)
        {
            _logger.Info($"Getting account users for ukprn: {ukprn}", providerId: ukprn);
            var result = await _orchestrator.GetAccountUsers(ukprn);

            _logger.Info($"Found {result.Count()} user accounts for ukprn: {ukprn}", providerId: ukprn);

            return Ok(result);
        }
    }
}