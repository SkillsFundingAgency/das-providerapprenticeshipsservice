using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/user")]
public class UserController : Controller
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
    //TODO [ApiAuthorize(Roles = "ReadUserSettings")]
    public async Task<IActionResult> GetUserSettings(string userRef)
    {
        _logger.Info($"Getting users settings for user: {userRef}");
        var result = await _orchestrator.GetUser(userRef);
        _logger.Info($"Found {(result == null ? "0" : "1")}  users settings for user: {userRef}");

        return Ok(result);
    }
}