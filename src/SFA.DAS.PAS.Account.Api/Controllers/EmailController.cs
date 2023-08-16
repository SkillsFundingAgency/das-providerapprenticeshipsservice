using SFA.DAS.PAS.Account.Api.Authorization;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Logging;

namespace SFA.DAS.PAS.Account.Api.Controllers;

[Authorize(Policy = ApiRoles.ReadAccountUsers)]
[Route("api/email")]
public class EmailController : Controller
{
    private readonly IEmailOrchestrator _emailOrchestrator;

    private readonly IProviderCommitmentsLogger _logger;

    public EmailController(IEmailOrchestrator emailOrchestrator, IProviderCommitmentsLogger logger)
    {
        _emailOrchestrator = emailOrchestrator;
        _logger = logger;
    }

    [HttpPost]
    [Route("{ukprn}/send")]
    public async Task<IActionResult> SendEmailToAllProviderRecipients([FromRoute]long ukprn, [FromBody] ProviderEmailRequest request)
    {
        try
        {
            await _emailOrchestrator.SendEmailToAllProviderRecipients(ukprn, request);
            _logger.Info($"Email template '{request?.TemplateId}' sent to Provider recipients successfully", ukprn);
            
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.Error(exception, $"Error sending email template '{request?.TemplateId}' to Provider recipients", ukprn);
            throw;
        }
    }
}