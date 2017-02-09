using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [RoutePrefix("{providerId}/apprentices")]
    public class CommitmentController : BaseController
    {
        private const string LastCohortPageSessionKey = "lastCohortPageSessionKey";

        private readonly CommitmentOrchestrator _commitmentOrchestrator;

        public CommitmentController(CommitmentOrchestrator commitmentOrchestrator)
        {
            if (commitmentOrchestrator == null)
                throw new ArgumentNullException(nameof(commitmentOrchestrator));

            _commitmentOrchestrator = commitmentOrchestrator;
        }

        [HttpGet]
        [Route("Cohorts")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> Cohorts(long providerId)
        {
            var model = await _commitmentOrchestrator.GetCohorts(providerId);
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
        [Route("WithEmployer")]
        public async Task<ActionResult> WithEmployer(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAllWithEmployer(providerId);

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("NewRequests")]
        public async Task<ActionResult> NewRequests(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAllNewRequests(providerId);
            Session[LastCohortPageSessionKey] = RequestStatus.NewRequest;

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("ReadyForReview")]
        public async Task<ActionResult> ReadyForReview(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAllReadyForReview(providerId);
            Session[LastCohortPageSessionKey] = RequestStatus.ReadyForReview;

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("ReadyForApproval")]
        public async Task<ActionResult> ReadyForApproval(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAllReadyForApproval(providerId);
            Session[LastCohortPageSessionKey] = RequestStatus.ReadyForApproval;

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Details", Name = "CohortDetails")]
        public async Task<ActionResult> Details(long providerId, string hashedCommitmentId)
        {
            var model = await _commitmentOrchestrator.GetCommitmentDetails(providerId, hashedCommitmentId);

            model.BackLinkUrl = GetReturnToListUrl(providerId);
            return View(model);
        }

        private string GetReturnToListUrl(long providerId)
        {
            switch (GetRequestStatusFromSession())
            {
                case RequestStatus.WithEmployerForApproval:
                case RequestStatus.SentForReview:
                    return Url.Action("WithEmployer", new { providerId });
                case RequestStatus.NewRequest:
                    return Url.Action("NewRequests", new { providerId });
                case RequestStatus.ReadyForReview:
                   return Url.Action("ReadyForReview", new { providerId });
                case RequestStatus.ReadyForApproval:
                   return Url.Action("ReadyForApproval", new { providerId });
                default:
                   return Url.Action("Cohorts", new { providerId });
            }
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
                return View(viewModel);
            }

            if (viewModel.DeleteConfirmed == null || !viewModel.DeleteConfirmed.Value)
            {   
                return RedirectToAction(
                    "Details",
                    new { providerId = viewModel.ProviderId, hashedCommitmentId = viewModel.HashedCommitmentId });
            }

            // ToDo: Delete
            SetInfoMessage("Cohort deleted");

            var currentStatusCohortAny = 
                await _commitmentOrchestrator.GetCohortsForCurrentStatus(viewModel.ProviderId, GetRequestStatusFromSession());

            if (!currentStatusCohortAny)
                return RedirectToAction("Cohorts", new { providerId = viewModel.ProviderId });

            return Redirect(GetReturnToListUrl(viewModel.ProviderId));
        }


        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Edit/{hashedApprenticeshipId}", Name = "EditApprenticeship")]
        public async Task<ActionResult> Edit(long providerId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var model = await _commitmentOrchestrator.GetApprenticeship(providerId, hashedCommitmentId, hashedApprenticeshipId);
            ViewBag.ApprenticeshipProgrammes = model.ApprenticeshipProgrammes;

            ViewBag.ApprovalWarningState = model.WarningValidation;

            return View(model.Apprenticeship);
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/Edit/{hashedApprenticeshipId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApprenticeshipViewModel apprenticeship)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return await RedisplayApprenticeshipView(apprenticeship);
                }

                await _commitmentOrchestrator.UpdateApprenticeship(apprenticeship);
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex);

                return await RedisplayApprenticeshipView(apprenticeship);
            }

            return RedirectToAction("Details", new { apprenticeship.ProviderId, apprenticeship.HashedCommitmentId });
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
                return RedirectToRoute("EditApprenticeship", new { providerId = viewModel.ProviderId, hashedCommitmentId = viewModel.HashedCommitmentId, hashedApprenticeshipId = viewModel.HashedApprenticeshipId });
            }

            var deletedApprenticeshipName = await _commitmentOrchestrator.DeleteApprenticeship(viewModel);
            SetInfoMessage($"Apprentice record for {deletedApprenticeshipName} deleted");

            return RedirectToRoute("CohortDetails", new { providerId = viewModel.ProviderId, hashedCommitmentId = viewModel.HashedCommitmentId });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/AddApprentice")]
        public async Task<ActionResult> Create(long providerId, string hashedCommitmentId)
        {
            var model = await _commitmentOrchestrator.GetCreateApprenticeshipViewModel(providerId, hashedCommitmentId);
            ViewBag.ApprenticeshipProgrammes = model.ApprenticeshipProgrammes;

            return View(model.Apprenticeship);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/AddApprentice")]
        public async Task<ActionResult> Create(ApprenticeshipViewModel apprenticeship)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return await RedisplayApprenticeshipView(apprenticeship);
                }

                await _commitmentOrchestrator.CreateApprenticeship(apprenticeship);
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex);

                return await RedisplayApprenticeshipView(apprenticeship);
            }

            return RedirectToAction("Details", new { apprenticeship.ProviderId, apprenticeship.HashedCommitmentId });
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
        // [OutputCache(CacheProfile = "NoCache")]
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
                await _commitmentOrchestrator.SubmitCommitment(viewModel.ProviderId, viewModel.HashedCommitmentId, viewModel.SaveStatus, string.Empty);
                return RedirectToAction("Approved", new { providerId = viewModel.ProviderId, hashedCommitmentId = viewModel.HashedCommitmentId });
            }

            return RedirectToAction("Cohorts", new { providerId = viewModel.ProviderId });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/RequestApproved")]
        public async Task<ActionResult> Approved(long providerId, string hashedCommitmentId)
        {
            var commitment = await _commitmentOrchestrator.GetCommitment(providerId, hashedCommitmentId);
            var currentStatusCohortAny = await _commitmentOrchestrator.GetCohortsForCurrentStatus(providerId, RequestStatus.ReadyForApproval);
            string url;
            var linkText = "Return to Approve cohorts";
            if (currentStatusCohortAny)
            {
                url = Url.Action("ReadyForApproval", new { ProviderId = providerId });
                TempData["FlashMessage"] = "Cohort approved";
            }
            else
            {
                url = Url.Action("Cohorts", "Commitment", new { ProviderId = providerId });
                linkText = "Return to Your cohorts";
            }

            var model = new AcknowledgementViewModel { CommitmentReference = commitment.Reference, EmployerName = commitment.LegalEntityName, ProviderName = commitment.ProviderName, Message = string.Empty, RedirectUrl = url, RedirectLinkText = linkText };

            return View("RequestApproved", model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> Submit(long providerId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            try
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
            catch (InvalidStateException)
            {
                return RedirectToAction("Cohorts", new { providerId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> Submit(SubmitCommitmentViewModel model)
        {
            await _commitmentOrchestrator.SubmitCommitment(model.ProviderId, model.HashedCommitmentId, model.SaveStatus, model.Message);

            return RedirectToAction("Acknowledgement", new { providerId = model.ProviderId, hashedCommitmentId = model.HashedCommitmentId, message = model.Message });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public async Task<ActionResult> Acknowledgement(long providerId, string hashedCommitmentId, string message)
        {
            var commitment = await _commitmentOrchestrator.GetCommitment(providerId, hashedCommitmentId);
            var currentStatusCohortAny = await _commitmentOrchestrator.GetCohortsForCurrentStatus(providerId, GetRequestStatusFromSession());
            var url = string.Empty;
            var linkText = "Go back to view cohorts";

            // ToDo: Simplify when we remove flash messages
            if (!currentStatusCohortAny)
            {
                url = Url.Action("Cohorts", "Commitment", new { ProviderId = providerId });
                linkText = "Return to Your cohorts";
            }
            else
            {
                switch (GetRequestStatusFromSession())
                {
                    case RequestStatus.ReadyForReview:
                        url = Url.Action("ReadyForReview", new { ProviderId = providerId });
                        SetTempMessage(commitment.Status);
                        break;
                    case RequestStatus.ReadyForApproval:
                        url = Url.Action("ReadyForApproval", new { ProviderId = providerId });
                        SetTempMessage(commitment.Status);
                        break;
                    case RequestStatus.NewRequest:
                        url = Url.Action("NewRequests", new { ProviderId = providerId });
                        SetTempMessage(commitment.Status);
                        break;
                }
            }

            return View(new AcknowledgementViewModel { CommitmentReference = commitment.Reference, EmployerName = commitment.LegalEntityName, ProviderName = commitment.ProviderName, Message = message, RedirectUrl = url, RedirectLinkText = linkText });
        }

        private RequestStatus GetRequestStatusFromSession()
        {
            var status = (RequestStatus?)Session[LastCohortPageSessionKey] ?? RequestStatus.None;
            return status;
        }

        private void SetTempMessage(RequestStatus newRequestStatus)
        {
            if (newRequestStatus == RequestStatus.SentForReview)
                TempData["FlashMessage"] = "Your cohort is with your employer for review";
            if (newRequestStatus == RequestStatus.WithEmployerForApproval)
                TempData["FlashMessage"] = "Your cohort is with your employer for approval";
        }

        private void AddErrorsToModelState(InvalidRequestException ex)
        {
            foreach (var error in ex.ErrorMessages)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        private async Task<ActionResult> RedisplayApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var model = await _commitmentOrchestrator.GetCreateApprenticeshipViewModel(apprenticeship.ProviderId, apprenticeship.HashedCommitmentId);
            model.Apprenticeship = apprenticeship;
            ViewBag.ApprenticeshipProgrammes = model.ApprenticeshipProgrammes;

            return View(model.Apprenticeship);
        }
    }
}
