using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ProviderUkPrnCheck]
    [RoutePrefix("{providerId}/apprentices/manage")]
    public class ManageApprenticesController : BaseController
    {
        private readonly ManageApprenticesOrchestrator _orchestrator;

        public ManageApprenticesController(ManageApprenticesOrchestrator orchestrator)
        {
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> Index(long providerId)
        {
            var model = await _orchestrator.GetApprenticeships(providerId);
            return View(model);
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> Details(long providerid, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetApprenticeship(providerid, hashedApprenticeshipId);
            return View(model);
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprovedApprentice")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> Edit(long providerid, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetApprenticeshipForEdit(providerid, hashedApprenticeshipId);
            ViewBag.ApprenticeshipProgrammes = model.ApprenticeshipProgrammes;
            return View(model.Apprenticeship);
        }

        [HttpPost]
        [Route("{hashedApprenticeshipId}/edit")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> Edit(long providerid, ApprenticeshipViewModel model)
        {
            var validationErrors = await _orchestrator.ValidateEditApprenticeship(model);

            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
            
            if (!ModelState.IsValid)
            {
                var viewModel = await _orchestrator.GetApprenticeshipForEdit(providerid, model.HashedApprenticeshipId);
                ViewBag.ApprenticeshipProgrammes = viewModel.ApprenticeshipProgrammes;
                return View(model);
            }

            return RedirectToAction("Confirm");
        }
    }
}