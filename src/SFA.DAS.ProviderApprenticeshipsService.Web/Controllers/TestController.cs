using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;


public class TestController : Controller
{
    private readonly ProviderApprenticeshipsDbContext _dbContext;
    private readonly ILogger<TestController> _logger;

    public TestController(ProviderApprenticeshipsDbContext dbContext, ILogger<TestController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        try
        {
            await _dbContext.Database.CanConnectAsync();
            _logger.LogWarning("DbContext connect success.");
        }
        catch (Exception e)
        {
            _logger.LogError("DbContext connect failure.", e);
        }

        return RedirectToAction("Index", "Home");
    }
}