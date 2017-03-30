using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
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
        [Route("{hashedApprenticeshipId}/confirm")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ConfirmChanges(long providerId, ApprenticeshipViewModel model)
        {
            var validationErrors = await _orchestrator.ValidateEditApprenticeship(model);

            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
            
            if (!ModelState.IsValid)
            {
                var viewModel = await _orchestrator.GetApprenticeshipForEdit(providerId, model.HashedApprenticeshipId);
                ViewBag.ApprenticeshipProgrammes = viewModel.ApprenticeshipProgrammes;
                return View("Edit", model);
            }

            var updateViewModel = await _orchestrator.GetConfirmChangesModel(providerId, model.HashedApprenticeshipId, model);

            if (!AnyChanges(updateViewModel))
            {
                //todo: put this in a method
                var viewModel = await _orchestrator.GetApprenticeshipForEdit(providerId, model.HashedApprenticeshipId);
                ModelState.AddModelError("NoChangesRequested", "No changes made");
                ViewBag.ApprenticeshipProgrammes = viewModel.ApprenticeshipProgrammes;
                return View("Edit", model);
            }

            return View(updateViewModel);
        }


        [HttpPost]
        [Route("{hashedApprenticeshipId}/submit")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> SubmitChanges(long providerid, string hashedApprenticeshipId, UpdateApprenticeshipViewModel updateApprenticeship, string originalApprenticeshipDecoded)
        {
            var originalApprenticeship = System.Web.Helpers.Json.Decode<Apprenticeship>(originalApprenticeshipDecoded);
            updateApprenticeship.OriginalApprenticeship = originalApprenticeship;

            if (!ModelState.IsValid)
            {
                return View("ConfirmChanges", updateApprenticeship);
            }

            if (updateApprenticeship.ChangesConfirmed != null && !updateApprenticeship.ChangesConfirmed.Value)
            {
                return RedirectToAction("Details", new { providerid, hashedApprenticeshipId });
            }

            await _orchestrator.CreateApprenticeshipUpdate(updateApprenticeship, providerid, CurrentUserId);

            SetInfoMessage($"You suggested changes to the record for {originalApprenticeship.FirstName} {originalApprenticeship.LastName}. The employer needs to approve these changes.", FlashMessageSeverityLevel.Okay);

            return RedirectToAction("Details", new { providerid, hashedApprenticeshipId });
        }

        private bool AnyChanges(UpdateApprenticeshipViewModel data)
        {
            return
                !string.IsNullOrWhiteSpace(data.ULN)
                || !string.IsNullOrWhiteSpace(data.FirstName)
                || !string.IsNullOrWhiteSpace(data.LastName)
                || data.DateOfBirth != null
                || !string.IsNullOrWhiteSpace(data.TrainingName)
                || data.StartDate != null
                || data.EndDate != null
                || data.Cost != null
                || !string.IsNullOrWhiteSpace(data.ProviderRef);
        }
    }
}