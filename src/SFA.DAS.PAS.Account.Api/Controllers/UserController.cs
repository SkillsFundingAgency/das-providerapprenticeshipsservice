﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PAS.Account.Api.Authorization;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Authorize(Policy = ApiRoles.ReadUserSettings)]
[Route("api/user")]
public class UserController : Controller
{
    private readonly IUserOrchestrator _orchestrator;

    private readonly IProviderCommitmentsLogger _logger;

    public UserController(IUserOrchestrator orchestrator, IProviderCommitmentsLogger logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    [Route("{userRef}")]
    [HttpGet]
    public async Task<IActionResult> GetUserSettings(string userRef)
    {
        _logger.Info($"Getting users settings for user: {userRef}");
        var result = await _orchestrator.GetUser(userRef);
        _logger.Info($"Found {(result == null ? "0" : "1")}  users settings for user: {userRef}");

        return Ok(result);
    }
}