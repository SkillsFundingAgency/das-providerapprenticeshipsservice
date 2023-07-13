using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;


public class TestController : Controller
{
    private readonly ProviderApprenticeshipsDbContext _dbContext;

    public TestController(ProviderApprenticeshipsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        try
        {
            await _dbContext.Database.CanConnectAsync();
            return Json("success");
        }
        catch (Exception e)
        {
            return Json(e);
        }
    }
}