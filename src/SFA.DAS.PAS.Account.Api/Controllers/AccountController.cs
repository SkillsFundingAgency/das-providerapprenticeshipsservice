using SFA.DAS.PAS.Account.Api.Authorization;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.Controllers;

[Authorize(Policy = ApiRoles.ReadAccountUsers)]
[Route("api/account")]
public class AccountController : Controller
{
    private readonly IAccountOrchestrator _orchestrator;

    private readonly IProviderCommitmentsLogger _logger;

    public AccountController(IAccountOrchestrator orchestrator, IProviderCommitmentsLogger logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    [Route("{ukprn}/users")]
    [HttpGet]
    public async Task<IActionResult> GetAccountUsers(long ukprn)
    {
        _logger.Info($"Getting account users for ukprn: {ukprn}", providerId: ukprn);
        var result = await _orchestrator.GetAccountUsers(ukprn);

        _logger.Info($"Found {result.Count()} user accounts for ukprn: {ukprn}", providerId: ukprn);

        return Ok(result);
    }

    [Route("{ukprn}/agreement")]
    [HttpGet]
    public async Task<IActionResult> GetAgreement(long ukprn)
    {
        _logger.Info($"Getting agreement for ukprn: {ukprn}", providerId: ukprn);
        var result = await _orchestrator.GetAgreement(ukprn);

        _logger.Info($"Ukprn: {ukprn} has agreement status: {result.Status}", providerId: ukprn);

        return Ok(result);
    }
}
