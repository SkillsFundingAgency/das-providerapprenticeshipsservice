using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ProviderUkPrnCheck]
    [RoutePrefix("{providerId}/apprentices/manage")]
    public class ManageApprenticesController : BaseController
    {
        private readonly ManageApprenticesOrchestrator _orchestrator;
        private readonly IFeatureToggleService _featureToggleService;
        private readonly ILinkGenerator _providerUrlHelper;

        public ManageApprenticesController(ManageApprenticesOrchestrator orchestrator, ICookieStorageService<FlashMessageViewModel> flashMessage, IFeatureToggleService featureToggleService, ILinkGenerator providerUrlHelper) : base(flashMessage)
        {
            _orchestrator = orchestrator;
            _featureToggleService = featureToggleService;
            _providerUrlHelper = providerUrlHelper;
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public ActionResult Index(long providerId)
        {
            return Redirect(_providerUrlHelper.ProviderCommitmentsLink($"{providerId}/apprentices"));
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public ActionResult Details(long providerid, string hashedApprenticeshipId)
        {
            return Redirect(_providerUrlHelper.ProviderCommitmentsLink($"{providerid}/apprentices/{hashedApprenticeshipId}"));
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprovedApprentice")]
        [RoleAuthorize(Roles = nameof(RoleNames.HasAccountOwnerPermission))]
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
        [RoleAuthorize(Roles = nameof(RoleNames.HasAccountOwnerPermission))]
        public async Task<ActionResult> ConfirmChanges(long providerId, ApprenticeshipViewModel model)
        {
            var updateViewModel = await _orchestrator.GetConfirmChangesModel(providerId, model.HashedApprenticeshipId, model);

            var validationErrors = await _orchestrator.ValidateEditApprenticeship(model, updateViewModel);

            foreach (var error in validationErrors)
            {
                if(ModelState.ContainsKey(error.Key)) continue;
                ModelState.AddModelError(error.Key, error.Value);
            }
            
            if (!ModelState.IsValid)
            {
                return await RedisplayEditApprenticeshipView(model);
            }


            if (!AnyChanges(updateViewModel))
            {
                ModelState.AddModelError("NoChangesRequested", "No changes made");
                return await RedisplayEditApprenticeshipView(model);
            }

            ModelState.Clear();
            return View(updateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/submit")]
        [RoleAuthorize(Roles = nameof(RoleNames.HasAccountOwnerPermission))]
        public async Task<ActionResult> SubmitChanges(long providerId, string hashedApprenticeshipId, CreateApprenticeshipUpdateViewModel updateApprenticeship)
        {
            var originalApp = await _orchestrator.GetApprenticeship(providerId, hashedApprenticeshipId);
            updateApprenticeship.OriginalApprenticeship = originalApp;

            if (!ModelState.IsValid)
            {
                return View("ConfirmChanges", updateApprenticeship);
            }

            if (updateApprenticeship.ChangesConfirmed != null && !updateApprenticeship.ChangesConfirmed.Value)
            {
                return RedirectToAction("Details", new { providerId, hashedApprenticeshipId });
            }

            await _orchestrator.CreateApprenticeshipUpdate(updateApprenticeship, providerId, CurrentUserId, GetSignedInUser());

            var message = NeedReapproval(updateApprenticeship) 
                ? "Suggested changes sent to employer for approval, where needed."
                : "Apprentice updated";

            SetInfoMessage(message, FlashMessageSeverityLevel.Okay);

            return RedirectToAction("Details", new { providerId, hashedApprenticeshipId });
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
        [RoleAuthorize(Roles = nameof(RoleNames.HasAccountOwnerPermission))]
        public async Task<ActionResult> ReviewChanges(long providerId, string hashedApprenticeshipId, ReviewApprenticeshipUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await ReviewChanges(providerId, hashedApprenticeshipId);
            }

            await _orchestrator.SubmitReviewApprenticeshipUpdate(providerId, hashedApprenticeshipId, CurrentUserId, viewModel.ApproveChanges.Value, GetSignedInUser());

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
        [RoleAuthorize(Roles = nameof(RoleNames.HasAccountOwnerPermission))]
        public async Task<ActionResult> UndoChanges(long providerId, string hashedApprenticeshipId, UndoApprenticeshipUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await UndoChanges(providerId, hashedApprenticeshipId);
            }

            if (viewModel.ConfirmUndo.HasValue && viewModel.ConfirmUndo.Value)
            {
                SetInfoMessage("Changes undone", FlashMessageSeverityLevel.Okay);
                await _orchestrator.SubmitUndoApprenticeshipUpdate(providerId, hashedApprenticeshipId, CurrentUserId, GetSignedInUser());
            }

            return RedirectToAction("Details", new { providerId, hashedApprenticeshipId });
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
                || !string.IsNullOrWhiteSpace(data.CourseName)
                || data.StartDate != null
                || data.EndDate != null
                || data.Cost != null
                || data.ProviderRef != null;
        }

        private bool NeedReapproval(CreateApprenticeshipUpdateViewModel model)
        {
            return
                   !string.IsNullOrEmpty(model.FirstName)
                || !string.IsNullOrEmpty(model.LastName)
                || model.DateOfBirth?.DateTime != null
                || !string.IsNullOrEmpty(model.CourseCode)
                || model.StartDate?.DateTime != null
                || model.EndDate?.DateTime != null
                || !string.IsNullOrEmpty(model.Cost)
                ;
        }
    }
}