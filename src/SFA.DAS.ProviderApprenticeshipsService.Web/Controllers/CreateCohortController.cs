using System.Web.Mvc;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ProviderUkPrnCheck]
    [RoutePrefix("{providerId}/apprentices")]
    public class CreateCohortController : BaseController
    {
        public CreateCohortController(ICookieStorageService<FlashMessageViewModel> flashMessage) : base(flashMessage)
        { 
        }

        [HttpGet]
        [Route("cohorts/create")]
        public ActionResult Create(long providerId)
        {
            return View("CreateCohort");
        }
    }
}



