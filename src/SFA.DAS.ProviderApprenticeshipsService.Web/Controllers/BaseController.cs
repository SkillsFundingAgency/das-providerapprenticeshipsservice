using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using System.Net;
using System.Web;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.App_Start;
using SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly IProviderCommitmentsLogger _logger;
        protected BaseController()
        {
            _logger = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<IProviderCommitmentsLogger>();
        }

        protected void SetInfoMessage(string message)
        {
            TempData["InfoMessage"] = message;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var providerIdKey = "providerId";
            if (filterContext.ActionParameters.ContainsKey(providerIdKey))
            {    
                var pid = filterContext.ActionParameters[providerIdKey];
                if ($"{pid}" == string.Empty)
                    ThrowException(new HttpException((int)HttpStatusCode.BadRequest, "Missing provider id"));

                var claimUkprn = GetClaimValue("http://schemas.portal.com/ukprn");

                if ($"{pid}" != claimUkprn)
                    ThrowException(new HttpException((int)HttpStatusCode.Forbidden, "Your account does not have sufficient privileges"));
            }
        }

        private void ThrowException(HttpException httpException)
        {
            _logger.Info($"Error: {httpException.GetType()}, HttpErrorCode: {httpException.GetHttpCode()}, Message: {httpException.Message}");
            throw httpException;
        }

        private string GetClaimValue(string claimKey)
        {
            var claimIdentity = ((ClaimsIdentity)HttpContext.User.Identity).Claims.FirstOrDefault(claim => claim.Type == claimKey);
            return claimIdentity != null ? claimIdentity.Value : string.Empty;
        }
    }
}
