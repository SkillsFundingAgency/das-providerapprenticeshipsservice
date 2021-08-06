using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using System.Security.Claims;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ProviderUkPrnCheck]
    [RoutePrefix("{providerId}/apprentices")]
    public class CommitmentController : BaseController
    {
        private const string LastCohortPageCookieKey = "sfa-das-providerapprenticeshipsservice-lastCohortPage";
        private readonly ICookieStorageService<string> _lastCohortCookieStorageService;
        private readonly IFeatureToggleService _featureToggleService;
        private readonly ProviderUrlHelper.ILinkGenerator _providerUrlhelper;

        private readonly CommitmentOrchestrator _commitmentOrchestrator;
        private readonly ILog _logger;

        public CommitmentController(CommitmentOrchestrator commitmentOrchestrator, ILog logger, ICookieStorageService<FlashMessageViewModel> flashMessage, 
            ICookieStorageService<string> lastCohortCookieStorageService, IFeatureToggleService featureToggleService, ProviderUrlHelper.LinkGenerator providerUrlhelper) : base(flashMessage)
        {
            _commitmentOrchestrator = commitmentOrchestrator;
            _logger = logger;
            _lastCohortCookieStorageService = lastCohortCookieStorageService;
            _featureToggleService = featureToggleService;
            _providerUrlhelper = providerUrlhelper;
        }

        [HttpGet]
        [Route("Cohorts")]
        [OutputCache(CacheProfile = "NoCache")]
        [Deprecated]
        public ActionResult Cohorts(long providerId)
        {
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerId}/unapproved"));
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("AgreementNotSigned")]
        public async Task<ActionResult> AgreementNotSigned(long providerId, string hashedCommitmentId, string redirectTo)
        {
            var model = await _commitmentOrchestrator.GetAgreementPage(providerId, hashedCommitmentId);
            model.RequestListUrl = Url.Action(redirectTo, new { providerId });

            if (model.IsSignedAgreement)
                return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerId}/unapproved/{hashedCommitmentId}/details"));

            return View(model);
        }

        [HttpGet]
        [Route("cohorts/employer")]
        [Deprecated]
        public ActionResult WithEmployer(long providerId)
        {
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerId}/unapproved/with-employer"));
        }

        [HttpGet]
        [Route("cohorts/transferfunded")]
        [Deprecated]
        public ActionResult TransferFunded(long providerId)
        {
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerId}/unapproved/with-transfer-sender"));
        }

        [HttpGet]
        [Route("cohorts/review")]
        [Deprecated]
        public ActionResult ReadyForReview(long providerId)
        {
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerId}/unapproved/review"));
        }
		
		[Route("cohorts/drafts")]
        [Deprecated]
        public ActionResult DraftList(long providerId)
        {
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerId}/unapproved/draft"));
        }

        [HttpGet]
        [Deprecated]
        [Route("{hashedCommitmentId}/Details", Name = "CohortDetails")]
        public ActionResult Details(long providerId, string hashedCommitmentId)
        {
            _logger.Info($"To track Apprentice V1 details UrlReferrer Request: {HttpContext.Request.UrlReferrer} Request to Page: {HttpContext.Request.RawUrl}");
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerId}/unapproved/{hashedCommitmentId}/details"));
        }

        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details/delete")]
        [RoleAuthorize(Roles = nameof(RoleNames.HasContributorOrAbovePermission))]
        [Deprecated]
        public async Task<ActionResult> DeleteCohort(long providerId, string hashedCommitmentId)
        {
            var model = await _commitmentOrchestrator.GetDeleteCommitmentModel(providerId, hashedCommitmentId);
            
            return View(model);
        }

        [HttpPost]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details/delete")]
        [RoleAuthorize(Roles = nameof(RoleNames.HasContributorOrAbovePermission))]
        [Deprecated]
        public async Task<ActionResult> DeleteCohort(DeleteCommitmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await _commitmentOrchestrator.GetDeleteCommitmentModel(viewModel.ProviderId, viewModel.HashedCommitmentId);
                return View(model);
            }

            if (viewModel.DeleteConfirmed == null || !viewModel.DeleteConfirmed.Value)
            {
                return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{viewModel.ProviderId}/unapproved/{viewModel.HashedCommitmentId}/details"));
            }

            await _commitmentOrchestrator.DeleteCommitment(CurrentUserId, viewModel.ProviderId, viewModel.HashedCommitmentId, GetSignedInUser());

            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{viewModel.ProviderId}/unapproved"));
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/View/{hashedApprenticeshipId}", Name = "ViewApprenticeship")]
        public ActionResult View(long providerId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink(
                $"{providerId}/unapproved/{hashedCommitmentId}/apprentices/{hashedApprenticeshipId}"));
        }

        [Route("{hashedCommitmentId}/{hashedApprenticeshipId}/Delete")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> DeleteConfirmation(long providerId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var viewModel = await _commitmentOrchestrator.GetDeleteConfirmationModel(providerId, hashedCommitmentId, hashedApprenticeshipId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/{hashedApprenticeshipId}/Delete")]
        public async Task<ActionResult> DeleteConfirmation(DeleteConfirmationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.DeleteConfirmed != null && !viewModel.DeleteConfirmed.Value)
            {
                return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{viewModel.ProviderId}/unapproved/{viewModel.HashedCommitmentId}/apprentices/{viewModel.HashedApprenticeshipId}/edit"));
            }

            var deletedApprenticeshipName = await _commitmentOrchestrator.DeleteApprenticeship(CurrentUserId, viewModel, GetSignedInUser());
            SetInfoMessage($"Apprentice record for {deletedApprenticeshipName} deleted", FlashMessageSeverityLevel.Okay);

            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{viewModel.ProviderId}/unapproved/{viewModel.HashedCommitmentId}/details"));
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/AddApprentice")]
        [RoleAuthorize(Roles = nameof(RoleNames.HasContributorOrAbovePermission))]
        public async Task<ActionResult> AddApprentice(long providerId, string hashedCommitmentId)
        {
            string nextPage;

            var hashedIds = await _commitmentOrchestrator.GetHashedIdsFromCommitment(providerId, hashedCommitmentId);
            nextPage = _providerUrlhelper.ReservationsLink($"{providerId}/reservations/{hashedIds.HashedLegalEntityId}/select?cohortReference={hashedCommitmentId}");
            if (hashedIds.HashedTransferSenderId != null)
            {
                nextPage += $"&transferSenderId={hashedIds.HashedTransferSenderId}";
            }

            return Redirect(nextPage);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Finished")]
        [RoleAuthorize(Roles = nameof(RoleNames.HasContributorWithApprovalOrAbovePermission))]
        public async Task<ActionResult> FinishEditing(long providerId, string hashedCommitmentId)
        {
            var viewModel = await _commitmentOrchestrator.GetFinishEditing(providerId, hashedCommitmentId);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/Finished")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = nameof(RoleNames.HasContributorWithApprovalOrAbovePermission))]
        public async Task<ActionResult> FinishEditing(FinishEditingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.SaveStatus.IsSend())
            {
                return RedirectToAction("Submit", new { viewModel.ProviderId, viewModel.HashedCommitmentId, viewModel.SaveStatus });
            }

            if (viewModel.SaveStatus == SaveStatus.Approve)
            {
                await _commitmentOrchestrator.SubmitCommitment(CurrentUserId, viewModel.ProviderId, viewModel.HashedCommitmentId, viewModel.SaveStatus, string.Empty, GetSignedInUser());
                return RedirectToAction("Approved", new { providerId = viewModel.ProviderId, hashedCommitmentId = viewModel.HashedCommitmentId });
            }

            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{viewModel.ProviderId}/unapproved"));
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/RequestApproved")]
        [RoleAuthorize(Roles = nameof(RoleNames.HasContributorWithApprovalOrAbovePermission))]
        public ActionResult Approved(long providerId, string hashedCommitmentId)
        {
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerId}/unapproved/{hashedCommitmentId}/Acknowledgement/"));
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Submit")]
        [RoleAuthorize(Roles = nameof(RoleNames.HasContributorWithApprovalOrAbovePermission))]
        public async Task<ActionResult> Submit(long providerId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            var commitment = await _commitmentOrchestrator.GetCommitmentCheckState(providerId, hashedCommitmentId);

            var model = new SubmitCommitmentViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId,
                EmployerName = commitment.LegalEntityName,
                SaveStatus = saveStatus
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Submit")]
        [RoleAuthorize(Roles = nameof(RoleNames.HasContributorWithApprovalOrAbovePermission))]
        public async Task<ActionResult> Submit(SubmitCommitmentViewModel model)
        {
            await _commitmentOrchestrator.SubmitCommitment(CurrentUserId, model.ProviderId, model.HashedCommitmentId, model.SaveStatus, model.Message, GetSignedInUser());

            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{model.ProviderId}/unapproved/{model.HashedCommitmentId}/Acknowledgement?saveStatus={model.SaveStatus}"));
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        [RoleAuthorize(Roles = nameof(RoleNames.HasContributorWithApprovalOrAbovePermission))]
        public  ActionResult Acknowledgement(long providerId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            return Redirect(_providerUrlhelper.ProviderCommitmentsLink($"{providerId}/unapproved/{hashedCommitmentId}/Acknowledgement?saveStatus={saveStatus}"));
        }

        private string GetReturnToListUrl(long providerId)
        {
            switch (GetRequestStatusFromCookie())
            {
                case RequestStatus.NewRequest:
                    return Url.Action("DraftList", new {providerId});
                case RequestStatus.WithEmployerForApproval:
                case RequestStatus.SentForReview:
                    return Url.Action("WithEmployer", new { providerId });
                case RequestStatus.ReadyForReview:
                case RequestStatus.ReadyForApproval:
                    return Url.Action("ReadyForReview", new { providerId });
                case RequestStatus.WithSenderForApproval:
                case RequestStatus.RejectedBySender:
                    return Url.Action("TransferFunded", new {providerId});
                default:
                    return Url.Action("Cohorts", new { providerId });
            }
        }

        private RequestStatus GetRequestStatusFromCookie()
        {
            var status = _lastCohortCookieStorageService.Get(LastCohortPageCookieKey);

            if (string.IsNullOrWhiteSpace(status))
            {
                return RequestStatus.None;
            }

            return (RequestStatus)Enum.Parse(typeof(RequestStatus), status);
        }

        private void SaveRequestStatusInCookie(RequestStatus status)
        {
            _lastCohortCookieStorageService.Delete(LastCohortPageCookieKey);
            _lastCohortCookieStorageService.Create(status.ToString(), LastCohortPageCookieKey);
        }

        private void AddFlashMessageToViewModel(ViewModelBase model)
        {
            var flashMessage = GetFlashMessageViewModelFromCookie();

            if (flashMessage != null)
            {
                model.FlashMessage = flashMessage;
            }
        }
    }
}

