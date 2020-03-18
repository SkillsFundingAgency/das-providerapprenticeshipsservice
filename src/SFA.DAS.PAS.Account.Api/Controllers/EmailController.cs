using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.PAS.Account.Api.Attributes;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.Controllers
{
    [RoutePrefix("api/email")]
    public class EmailController : ApiController
    {
        private readonly EmailOrchestrator _emailOrchestrator;

        private readonly IProviderCommitmentsLogger _logger;

        public EmailController(EmailOrchestrator emailOrchestrator, IProviderCommitmentsLogger logger)
        {
            _emailOrchestrator = emailOrchestrator;
            _logger = logger;
        }

        [Route("{ukprn}/send")]
        [HttpPost]
        [ApiAuthorize(Roles = "ReadAccountUsers")]
        public async Task<IHttpActionResult> SendEmailToAllProviderRecipients(long ukprn, ProviderEmailRequest request)
        {
            await _emailOrchestrator.SendEmailToAllProviderRecipients(ukprn, request);
            return Ok();
        }
    }
}