using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PAS.Account.Api.Authorization;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System;
using System.Threading.Tasks;
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

    [Route("{ukprn}/send")]
    [HttpPost]
    public async Task<IActionResult> SendEmailToAllProviderRecipients(long ukprn, ProviderEmailRequest request)
    {
        try
        {
            await _emailOrchestrator.SendEmailToAllProviderRecipients(ukprn, request);
            _logger.Info($"Email template '{request?.TemplateId}' sent to Provider recipients successfully", ukprn);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.Error(e, $"Error sending email template '{request?.TemplateId}' to Provider recipients", ukprn);
            throw;
        }
    }
}