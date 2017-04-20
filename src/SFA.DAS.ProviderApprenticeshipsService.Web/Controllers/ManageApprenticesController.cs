using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
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
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/confirm")]
        public async Task<ActionResult> ConfirmChanges(long providerId, ApprenticeshipViewModel model)
        {
            var validationErrors = await _orchestrator.ValidateEditApprenticeship(model);

            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
            
            if (!ModelState.IsValid)
            {
                return await RedisplayEditApprenticeshipView(model);
            }

            var updateViewModel = await _orchestrator.GetConfirmChangesModel(providerId, model.HashedApprenticeshipId, model);

            if (!AnyChanges(updateViewModel))
            {
                ModelState.AddModelError("NoChangesRequested", "No changes made");
                return await RedisplayEditApprenticeshipView(model);
            }

            return View(updateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/submit")]
        public async Task<ActionResult> SubmitChanges(long providerid, string hashedApprenticeshipId, CreateApprenticeshipUpdateViewModel updateApprenticeship, string originalApprenticeshipDecoded)
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

        [HttpGet]
        [Route("{hashedApprenticeshipId}/review", Name = "ReviewApprovedApprenticeChange")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ReviewChanges(long providerId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetReviewApprenticeshipUpdateModel(providerId, hashedApprenticeshipId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/review")]
        public async Task<ActionResult> ReviewChanges(long providerId, string hashedApprenticeshipId, ReviewApprenticeshipUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await ReviewChanges(providerId, hashedApprenticeshipId);
            }

            await _orchestrator.SubmitReviewApprenticeshipUpdate(providerId, hashedApprenticeshipId, CurrentUserId, viewModel.ApproveChanges.Value);

            SetInfoMessage(viewModel.ApproveChanges.Value ? "Record updated" : "Changes rejected",
                FlashMessageSeverityLevel.Okay);

            return RedirectToAction("Details", new { providerId, hashedApprenticeshipId});
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/undo", Name = "UndoApprovedApprenticeChange")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> UndoChanges(long providerId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetUndoApprenticeshipUpdateModel(providerId, hashedApprenticeshipId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/undo")]
        public async Task<ActionResult> UndoChanges(long providerId, string hashedApprenticeshipId, UndoApprenticeshipUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await UndoChanges(providerId, hashedApprenticeshipId);
            }

            if (viewModel.ConfirmUndo.HasValue && viewModel.ConfirmUndo.Value)
            {
                SetInfoMessage("Changes undone", FlashMessageSeverityLevel.Okay);
                await _orchestrator.SubmitUndoApprenticeshipUpdate(providerId, hashedApprenticeshipId, CurrentUserId);
            }

            return RedirectToAction("Details", new { providerId, hashedApprenticeshipId });
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/datalock", Name = "DataLockDetails")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> DataLock(long providerId, string hashedApprenticeshipId)
        {
            return View();
        }

        private async Task<ActionResult> RedisplayEditApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var viewModel = await _orchestrator.GetApprenticeshipForEdit(apprenticeship.ProviderId, apprenticeship.HashedApprenticeshipId);
            ViewBag.ApprenticeshipProgrammes = viewModel.ApprenticeshipProgrammes;
            return View("Edit", apprenticeship);
        }
        private bool AnyChanges(CreateApprenticeshipUpdateViewModel data)
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