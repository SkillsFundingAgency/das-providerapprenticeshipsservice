using System.Web.Mvc;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    using System;

    public class ErrorController : BaseController
    {
        public ViewResult BadRequest()
        {
            Response.StatusCode = 400;

            return View("_Error400");
        }

        public ViewResult Forbidden()
        {
            Response.StatusCode = 403;

            return View("_Error403");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;

            return View("_Error404");
        }

        public ViewResult InternalServerError(Exception ex)
        {
            Response.StatusCode = 500;
            return View("_Error500");
        }
    }
}