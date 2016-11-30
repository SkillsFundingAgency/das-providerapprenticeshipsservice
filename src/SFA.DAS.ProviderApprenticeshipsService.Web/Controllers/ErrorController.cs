using System;
using System.Web.Mvc;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult BadRequest()
        {
            return View("_Error400");
        }

        public ViewResult AccessDenied()
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
    }
}
