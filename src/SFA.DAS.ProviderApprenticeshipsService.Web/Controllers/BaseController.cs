using System.Web;
using System.Web.Mvc;

using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is InvalidStateException)
            {
                var msg= HttpUtility.UrlEncode("You have been redirected from a page that is no longer accessible");
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("Index", "Account",
                    new { message =  msg });
            }
        }
    }
}