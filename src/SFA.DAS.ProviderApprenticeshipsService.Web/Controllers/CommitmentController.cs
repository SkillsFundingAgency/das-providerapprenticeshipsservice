using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application;
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
        [Route("{hashedCommitmentId}/Details")]
        public async Task<ActionResult> Details(long providerId, string hashedCommitmentId)
        {
            var model = await _commitmentOrchestrator.GetCommitmentDetails(providerId, hashedCommitmentId);

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Edit/{hashedApprenticeshipId}")]
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
                return RedirectToAction("Submit", 
                    new { viewModel.ProviderId, viewModel.HashedCommitmentId, viewModel.SaveStatus });
            }

            if(viewModel.SaveStatus == SaveStatus.Approve)
            {
                await _commitmentOrchestrator.SubmitCommitment(viewModel.ProviderId, viewModel.HashedCommitmentId, viewModel.SaveStatus, string.Empty);
                return RedirectToAction("Approved", new { providerId = viewModel.ProviderId, hashedCommitmentId = viewModel.HashedCommitmentId });
            }

            return RedirectToAction("Cohorts", new {providerId = viewModel.ProviderId});
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/RequestApproved")]
        public async Task<ActionResult> Approved(long providerId, string hashedCommitmentId)
        {
            var commitment = await _commitmentOrchestrator.GetCommitment(providerId, hashedCommitmentId);
            var url = Url.Action("ReadyForApproval", new { ProviderId = providerId });

            TempData["FlashMessage"] = "Cohort approved";
            var model = new AcknowledgementViewModel
                            {
                                CommitmentReference = commitment.Reference,
                                EmployerName = commitment.LegalEntityName,
                                ProviderName = commitment.ProviderName,
                                Message = string.Empty,
                                RedirectUrl = url
                            };

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
            await _commitmentOrchestrator.SubmitCommitment(model.ProviderId, model.HashedCommitmentId, model.SaveStatus, model.Message);

            return RedirectToAction("Acknowledgement", new
            {
                providerId = model.ProviderId,
                hashedCommitmentId = model.HashedCommitmentId,
                message = model.Message
            });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public async Task<ActionResult> Acknowledgement(long providerId, string hashedCommitmentId, string message)
        {
            var commitment = await _commitmentOrchestrator.GetCommitment(providerId, hashedCommitmentId);
            var url = string.Empty;
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
                    url = Url.Action("NewRequests", new { ProviderId = providerId } );
                    SetTempMessage(commitment.Status);
                    break;
            }

            return View(new AcknowledgementViewModel
            {
                CommitmentReference = commitment.Reference,
                EmployerName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Message = message,
                RedirectUrl = url
            });
        }

        private RequestStatus GetRequestStatusFromSession()
        {

            var status = (RequestStatus?)Session[LastCohortPageSessionKey] ?? RequestStatus.None;
            return status;
        }

        private void SetTempMessage(RequestStatus newRequestStatus)
        {
            if (newRequestStatus == RequestStatus.SentForReview)
                TempData["FlashMessage"] = "Your cohort is with your training provider for review";
            if (newRequestStatus == RequestStatus.WithEmployerForApproval)
                TempData["FlashMessage"] = "Your cohort is with your training provider for approval";
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
