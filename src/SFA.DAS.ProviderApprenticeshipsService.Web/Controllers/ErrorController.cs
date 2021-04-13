using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    using System;

    [AllowAllRoles]
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public ViewResult BadRequest()
        {
            return View("_Error400");
        }

        [Route("error/403")]
        public ViewResult Forbidden()
        {
            return View("_Error403");
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
}