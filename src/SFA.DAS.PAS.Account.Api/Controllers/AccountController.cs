using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.PAS.Account.Api.Attributes;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

//todo: looks like automapper package is not used and can be removed

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
        [ApiAuthorize(Roles = "ReadAccountUsers")]
        public async Task<IHttpActionResult> GetAccountUsers(long ukprn)
        {
            _logger.Info($"Getting account users for ukprn: {ukprn}", providerId: ukprn);
            var result = await _orchestrator.GetAccountUsers(ukprn);

            _logger.Info($"Found {result.Count()} user accounts for ukprn: {ukprn}", providerId: ukprn);

            return Ok(result);
        }

        [Route("{ukprn}/agreement")]
        [HttpGet]
        [ApiAuthorize(Roles = "ReadAccountUsers")]
        public async Task<IHttpActionResult> GetAgreement(long ukprn)
        {
            _logger.Info($"Getting agreement for ukprn: {ukprn}", providerId: ukprn);
            var result = await _orchestrator.GetAgreement(ukprn);

            _logger.Info($"Ukprn: {ukprn} has agreement status: {result.Status}", providerId: ukprn);

            return Ok(result);
        }
    }
}