﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using System.Security.Claims;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

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
        public async Task<ActionResult> Cohorts(long providerId)
        {
            var model = await _commitmentOrchestrator.GetCohorts(providerId);

            AddFlashMessageToViewModel(model);

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("AgreementNotSigned")]
        public async Task<ActionResult> AgreementNotSigned(long providerId, string hashedCommitmentId, string redirectTo)
        {
            var model = await _commitmentOrchestrator.GetAgreementPage(providerId, hashedCommitmentId);
            model.RequestListUrl = Url.Action(redirectTo, new { providerId });

            if (model.IsSignedAgreement)
                return RedirectToAction("Details", new { providerId, hashedCommitmentId });

            return View(model);
        }

        [HttpGet]
        [Route("cohorts/employer")]
        public async Task<ActionResult> WithEmployer(long providerId)
        {
            SaveRequestStatusInCookie(RequestStatus.WithEmployerForApproval);

            var model = await _commitmentOrchestrator.GetAllWithEmployer(providerId);

            AddFlashMessageToViewModel(model);

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/transferfunded")]
        public async Task<ActionResult> TransferFunded(long providerId)
        {
            SaveRequestStatusInCookie(RequestStatus.WithSenderForApproval);

            var model = await _commitmentOrchestrator.GetAllTransferFunded(providerId);

            AddFlashMessageToViewModel(model);

            return View(model);
        }


        [HttpGet]
        [Route("cohorts/review")]
        public async Task<ActionResult> ReadyForReview(long providerId)
        {           
            SaveRequestStatusInCookie(RequestStatus.ReadyForReview);

            var model = await _commitmentOrchestrator.GetAllReadyForReview(providerId);

            AddFlashMessageToViewModel(model);

            return View("RequestList", model);
        }
		
		        [Route("cohorts/drafts")]
        public async Task<ActionResult> DraftList(long providerId)
        {
            SaveRequestStatusInCookie(RequestStatus.NewRequest);

            var model = await _commitmentOrchestrator.GetAllDrafts(providerId);

            AddFlashMessageToViewModel(model);

            return View("DraftList", model);
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Details", Name = "CohortDetails")]
        public async Task<ActionResult> Details(long providerId, string hashedCommitmentId)
        {
            LogUserClaims();

            var model = await _commitmentOrchestrator.GetCommitmentDetails(providerId, hashedCommitmentId);
               
            model.BackLinkUrl = GetReturnToListUrl(providerId);
             
            AddFlashMessageToViewModel(model);

            return View(model);
        }

        private void LogUserClaims()
        {
            var claims = ((ClaimsIdentity)HttpContext.User.Identity).Claims
                        .Select(x => $"{x.Type}: {x.Value}").ToArray();

            var logValue = string.Join(Environment.NewLine, claims);

            _logger.Trace("User claims", new Dictionary<string, object> { { "providerClaims", logValue } });
        }

        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details/delete")]
        public async Task<ActionResult> DeleteCohort(long providerId, string hashedCommitmentId)
        {
            var model = await _commitmentOrchestrator.GetDeleteCommitmentModel(providerId, hashedCommitmentId);
            
            return View(model);
        }

        [HttpPost]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details/delete")]
        public async Task<ActionResult> DeleteCohort(DeleteCommitmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await _commitmentOrchestrator.GetDeleteCommitmentModel(viewModel.ProviderId, viewModel.HashedCommitmentId);
                return View(model);
            }

            if (viewModel.DeleteConfirmed == null || !viewModel.DeleteConfirmed.Value)
            {   
                return RedirectToAction(
                    "Details",
                    new { providerId = viewModel.ProviderId, hashedCommitmentId = viewModel.HashedCommitmentId });
            }

            await _commitmentOrchestrator.DeleteCommitment(CurrentUserId, viewModel.ProviderId, viewModel.HashedCommitmentId, GetSignedInUser());

            SetInfoMessage("Cohort deleted", FlashMessageSeverityLevel.Okay);

            var currentStatusCohortAny = 
                await _commitmentOrchestrator.AnyCohortsForStatus(viewModel.ProviderId, GetRequestStatusFromCookie());

            if (!currentStatusCohortAny)
                return RedirectToAction("Cohorts", new { providerId = viewModel.ProviderId });

            return Redirect(GetReturnToListUrl(viewModel.ProviderId));
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/View/{hashedApprenticeshipId}", Name = "ViewApprenticeship")]
        public async Task<ActionResult> View(long providerId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var model = await _commitmentOrchestrator.GetApprenticeshipViewModel(providerId, hashedCommitmentId, hashedApprenticeshipId);
            return View(model);
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

            return RedirectToRoute("CohortDetails", new { providerId = viewModel.ProviderId, hashedCommitmentId = viewModel.HashedCommitmentId });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/AddApprentice")]
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
        public async Task<ActionResult> FinishEditing(long providerId, string hashedCommitmentId)
        {
            var viewModel = await _commitmentOrchestrator.GetFinishEditing(providerId, hashedCommitmentId);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/Finished")]
        [ValidateAntiForgeryToken]
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

            if (viewModel.SaveStatus == SaveStatus.Save)
            {
                SetInfoMessage("Cohort saved but not sent" ,FlashMessageSeverityLevel.None );
                var currentStatusCohortAny = await _commitmentOrchestrator.AnyCohortsForStatus(viewModel.ProviderId, GetRequestStatusFromCookie());
                if (currentStatusCohortAny)
                    return Redirect(GetReturnToListUrl(viewModel.ProviderId));
            }

            return RedirectToAction("Cohorts", new { providerId = viewModel.ProviderId });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/RequestApproved")]
        public async Task<ActionResult> Approved(long providerId, string hashedCommitmentId)
        {
            var model = await _commitmentOrchestrator.GetApprovedViewModel(providerId, hashedCommitmentId);
            return View("RequestApproved", model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Submit")]
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
        public async Task<ActionResult> Submit(SubmitCommitmentViewModel model)
        {
            await _commitmentOrchestrator.SubmitCommitment(CurrentUserId, model.ProviderId, model.HashedCommitmentId, model.SaveStatus, model.Message, GetSignedInUser());

            return RedirectToAction("Acknowledgement", new { providerId = model.ProviderId, hashedCommitmentId = model.HashedCommitmentId, saveStatus = model.SaveStatus});
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public async Task<ActionResult> Acknowledgement(long providerId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            var viewModel = await _commitmentOrchestrator.GetAcknowledgementViewModel(providerId, hashedCommitmentId, saveStatus);

            var currentStatusCohortAny = await _commitmentOrchestrator.AnyCohortsForStatus(providerId, GetRequestStatusFromCookie());
            var url = GetReturnToListUrl(providerId);
            var linkText = "Go back to view cohorts";
            if (!currentStatusCohortAny)
            {
                linkText = "Return to Your cohorts";
                url = Url.Action("Cohorts", new { providerId });
            }

            viewModel.RedirectUrl = url;
            viewModel.RedirectLinkText = linkText;

            return View(viewModel);
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

