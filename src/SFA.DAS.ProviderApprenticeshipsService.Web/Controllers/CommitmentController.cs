using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ProviderUkPrnCheck]
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
        [Route("cohorts/employer")]
        public async Task<ActionResult> WithEmployer(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAllWithEmployer(providerId);

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/new")]
        public async Task<ActionResult> NewRequests(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAllNewRequests(providerId);
            Session[LastCohortPageSessionKey] = RequestStatus.NewRequest;

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/review")]
        public async Task<ActionResult> ReadyForReview(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAllReadyForReview(providerId);
            Session[LastCohortPageSessionKey] = RequestStatus.ReadyForReview;

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/approve")]
        public async Task<ActionResult> ReadyForApproval(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAllReadyForApproval(providerId);
            Session[LastCohortPageSessionKey] = RequestStatus.ReadyForApproval;

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/verification")]
        public async Task<ActionResult> VerificationOfEmployer(long providerId, string hashedCommitmentId)
        {
            var model = await _commitmentOrchestrator.GetVerificationOfEmployer(providerId, hashedCommitmentId);
            return View(model);
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/verification")]
        [ValidateAntiForgeryToken]
        public ActionResult VerificationOfEmployer(VerificationOfEmployerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.ConfirmProvisionOfTrainingForOrganisation.Value)
            {
                return RedirectToAction("VerificationOfRelationship", new { viewModel.ProviderId, viewModel.HashedCommitmentId });
            }
            else
            {
                return RedirectToAction("VerificationStopped");
            }
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/verification-stopped")]
        public ActionResult VerificationStopped(long providerId, string hashedCommitmentId)
        {
            return View();
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/verification-relationship")]
        public async Task<ActionResult> VerificationOfRelationship(long providerId, string hashedCommitmentId)
        {
            var model = await _commitmentOrchestrator.GetVerificationOfRelationship(providerId, hashedCommitmentId);
            return View(model);
        }


        [HttpPost]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/verification-relationship")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerificationOfRelationship(VerificationOfRelationshipViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await _commitmentOrchestrator.VerifyRelationship(viewModel.ProviderId, viewModel.HashedCommitmentId, viewModel.OrganisationIsSameOrConnected.Value, CurrentUserId);

            return RedirectToAction("Details", new { viewModel.ProviderId, viewModel.HashedCommitmentId });
        }


        [HttpGet]
        [Route("{hashedCommitmentId}/Details", Name = "CohortDetails")]
        public async Task<ActionResult> Details(long providerId, string hashedCommitmentId)
        {
            var model = await _commitmentOrchestrator.GetCommitmentDetails(providerId, hashedCommitmentId);

            if (!model.RelationshipVerified)
            {
                return RedirectToAction("VerificationOfEmployer", new {providerId, hashedCommitmentId});
            }

            var groupes = model.ApprenticeshipGroups.Where(m => m.ShowOverlapError);
            foreach (var groupe in groupes)
            {
                var errorMessage = groupe.TrainingProgramme?.Title != null 
                    ? $"{groupe.TrainingProgramme?.Title ?? ""} apprentices training dates"
                    : "Apprentices training dates";

                ModelState.AddModelError($"{groupe.GroupId}", errorMessage);
            }
            model.BackLinkUrl = GetReturnToListUrl(providerId);
            return View(model);
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
            AddErrorsToModelState(model.ValidationErrors);
            
            ViewBag.ApprenticeshipProgrammes = model.ApprenticeshipProgrammes;

            return View(model.Apprenticeship);
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/Edit/{hashedApprenticeshipId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApprenticeshipViewModel apprenticeship)
        {
            try
            {
                AddErrorsToModelState(await _commitmentOrchestrator.ValidateApprenticeship(apprenticeship));
                if (!ModelState.IsValid)
                {
                    return await RedisplayApprenticeshipView(apprenticeship);
                }

                await _commitmentOrchestrator.UpdateApprenticeship(CurrentUserId, apprenticeship, GetSignedInUser());
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

            var deletedApprenticeshipName = await _commitmentOrchestrator.DeleteApprenticeship(CurrentUserId, viewModel, GetSignedInUser());
            SetInfoMessage($"Apprentice record for {deletedApprenticeshipName} deleted", FlashMessageSeverityLevel.Okay);

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
                AddErrorsToModelState(await _commitmentOrchestrator.ValidateApprenticeship(apprenticeship));
                if (!ModelState.IsValid)
                {
                    return await RedisplayApprenticeshipView(apprenticeship);
                }

                await _commitmentOrchestrator.CreateApprenticeship(CurrentUserId, apprenticeship, GetSignedInUser());
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
                TempData["FlashMessage"] = "Cohort saved but not sent";
                var currentStatusCohortAny = await _commitmentOrchestrator.GetCohortsForCurrentStatus(viewModel.ProviderId, GetRequestStatusFromSession());
                if (currentStatusCohortAny)
                    return Redirect(GetReturnToListUrl(viewModel.ProviderId));
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
            }
            else
            {
                url = Url.Action("Cohorts", "Commitment", new { ProviderId = providerId });
                linkText = "Return to your cohorts";
            }

            var model = new AcknowledgementViewModel { CommitmentReference = commitment.Reference, EmployerName = commitment.LegalEntityName, ProviderName = commitment.ProviderName, Message = string.Empty, RedirectUrl = url, RedirectLinkText = linkText };

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

            var currentStatusCohortAny = await _commitmentOrchestrator.GetCohortsForCurrentStatus(providerId, GetRequestStatusFromSession());
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

        private RequestStatus GetRequestStatusFromSession()
        {
            var status = (RequestStatus?)Session[LastCohortPageSessionKey] ?? RequestStatus.None;
            return status;
        }

        private void AddErrorsToModelState(InvalidRequestException ex)
        {
            foreach (var error in ex.ErrorMessages)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        private void AddErrorsToModelState(Dictionary<string, string> dict)
        {
            foreach (var error in dict)
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

