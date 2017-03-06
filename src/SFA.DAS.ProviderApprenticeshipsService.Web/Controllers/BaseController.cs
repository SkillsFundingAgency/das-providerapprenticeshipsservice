using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.App_Start;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly IProviderCommitmentsLogger _logger;
        protected string CurrentUserId = null;
        private const string RequiredUserRole = "DAA";

        protected BaseController()
        {
            _logger = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<IProviderCommitmentsLogger>();
        }

        protected void SetInfoMessage(string messageText, FlashMessageSeverityLevel level)
        {
            var message = new FlashMessageViewModel
            {
                Message = messageText,
                Severity = level
            };

            var flashMessage = JsonConvert.SerializeObject(message);
            TempData["InfoMessage"] = flashMessage;
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is InvalidStateException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("InvalidState", "Error");
            }
        }
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            const string ProviderIdKey = "providerId";

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                CurrentUserId = GetClaimValue(ClaimTypes.Upn);

                if (!HasClaimValue("http://schemas.portal.com/service", RequiredUserRole))
                {
                    _logger.Info($"HttpError 403, User does not have {RequiredUserRole} claim");
                    throw new HttpException((int)HttpStatusCode.Forbidden, $"Missing {RequiredUserRole} claim");
                }
            }

            if (filterContext.ActionParameters.ContainsKey(ProviderIdKey))
            {    
                var providerIdFromAction = filterContext.ActionParameters[ProviderIdKey];
                if ($"{providerIdFromAction}" == string.Empty)
                {
                    _logger.Info($"HttpError 400, Provider: {providerIdFromAction}, Resources");
                    throw new HttpException((int)HttpStatusCode.BadRequest, "Missing provider id");
                }

                var claimUkprn = GetClaimValue("http://schemas.portal.com/ukprn");

                if ($"{providerIdFromAction}" != claimUkprn)
                { 
                    _logger.Info($"HttpError 403, User ukprn: {providerIdFromAction}, Resources ukprn: {claimUkprn}");
                    throw new HttpException((int)HttpStatusCode.Forbidden, "Your account does not have sufficient privileges");
                }
            }
        }

        protected string GetClaimValue(string claimKey)
        {
            var claimIdentity = ((ClaimsIdentity)HttpContext.User.Identity).Claims.FirstOrDefault(claim => claim.Type == claimKey);
            return claimIdentity != null ? claimIdentity.Value : string.Empty;
        }

        protected bool HasClaimValue(string claimType, string value)
        {
            return
                ((ClaimsIdentity) HttpContext.User.Identity).Claims.Any(
                    x => x.Type == claimType && x.Value == value);
        }
    }
}
