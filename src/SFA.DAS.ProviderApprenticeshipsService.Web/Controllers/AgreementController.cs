using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(ProviderUkPrnCheckActionFilter))]
    [Route("{providerId}/agreements")]
    public class AgreementController : BaseController
    {
        private readonly IAgreementOrchestrator _orchestrator;

        public AgreementController(IAgreementOrchestrator orchestrator, ICookieStorageService<FlashMessageViewModel> flashMessage) : base(flashMessage)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("", Name = RouteNames.GetAgreements)]
        public async Task<IActionResult> Agreements(long providerId, string organisation = "")
        {
            var model = await _orchestrator.GetAgreementsViewModel(providerId, organisation);
            return View(model);
        }
    }
}