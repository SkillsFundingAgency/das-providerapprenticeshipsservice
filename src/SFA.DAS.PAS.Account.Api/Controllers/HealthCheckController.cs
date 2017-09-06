using System.Web.Http;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.Controllers
{
    public class HealthCheckController : ApiController
    {
        private readonly IProviderCommitmentsLogger _logger;

        public HealthCheckController(IProviderCommitmentsLogger logger)
        {
            _logger = logger;
        }

        [Route("api/HealthCheck")]
        public IHttpActionResult GetStatus()
        {
            _logger.Info("Called HealthCheck for Provider Account API");
            return Ok();
        }
    }
}