using System.Web.Mvc;

using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class BaseController : Controller
    {
        protected void SetInfoMessage(string message)
        {
            TempData["InfoMessage"] = message;
        }
        
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is InvalidStateException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("InvalidState", "Error");
            }
        }
    }
}
