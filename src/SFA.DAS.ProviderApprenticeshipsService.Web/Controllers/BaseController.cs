using System;
using System.Web.Mvc;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected void SetInfoMessage(string message)
        {
            TempData["InfoMessage"] = message;
        }
    }
}
