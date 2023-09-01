using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Error;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;

[AllowAnonymous]
public class ErrorController : Controller
{
    private readonly ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipsServiceConfiguration;
    private readonly IConfiguration _configuration;

    public ErrorController(
        ProviderApprenticeshipsServiceConfiguration providerApprenticeshipsServiceConfiguration,
        IConfiguration configuration)
    {
        _providerApprenticeshipsServiceConfiguration = providerApprenticeshipsServiceConfiguration;
        _configuration = configuration;
    }


    public ViewResult BadRequest()
    {
        return View("_Error400");
    }

    [Route("error/403")]
    public ViewResult Forbidden()
    {
        return View("_Error403", new Error403ViewModel(_configuration["ResourceEnvironmentName"])
        {
            UseDfESignIn = _providerApprenticeshipsServiceConfiguration.UseDfESignIn
        });
    }

    public ViewResult NotFound()
    {
        return View("_Error404");
    }

    public ViewResult InternalServerError(Exception ex)
    {
        return View("_Error500");
    }

    public ActionResult InvalidState()
    {
        return View("_InvalidState");
    }
}