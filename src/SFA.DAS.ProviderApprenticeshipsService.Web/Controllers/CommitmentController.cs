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
    public class CommitmentController : Controller
    {
        private readonly CommitmentOrchestrator _commitmentOrchestrator;

        public CommitmentController(CommitmentOrchestrator commitmentOrchestrator)
        {
            if (commitmentOrchestrator == null)
                throw new ArgumentNullException(nameof(commitmentOrchestrator));
            _commitmentOrchestrator = commitmentOrchestrator;
        }

        [HttpGet]
        [Route("WithEmployer")]
        public async Task<ActionResult> WithEmployer(long providerId)
        {
            // Filter by with employer
            var model = await _commitmentOrchestrator.GetAllWithEmployer(providerId);

            return View("RequestList", model);
        }


        [HttpGet]
        [Route("NewRequests")]
        public async Task<ActionResult> NewRequests(long providerId)
        {
            // Filter by new requests
            var model = await _commitmentOrchestrator.GetAllNewRequests(providerId);

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("ReadyForReview")]
        public async Task<ActionResult> ReadyForReview(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAllReadyForReview(providerId);

            return View("RequestList", model);
        }

        [HttpGet]
        [Route("ReadyForApproval")]
        public async Task<ActionResult> ReadyForApproval(long providerId)
        {
            // Filter by ready for approval
            var model = await _commitmentOrchestrator.GetAllReadyForApproval(providerId);

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
        [Route("{hashedCommitmentId}/Finished")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> FinishEditing(long providerId, string hashedCommitmentId)
        {
            var viewModel = await _commitmentOrchestrator.GetFinishEditing(providerId, hashedCommitmentId);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/Finished")]
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

                return RedirectToAction("Cohorts", new { providerId = viewModel.ProviderId });
            }

            return RedirectToAction("Cohorts", new {providerId = viewModel.ProviderId});
        }

        [HttpGet]
        public async Task<ActionResult> Submit(long providerId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            var commitment = await _commitmentOrchestrator.GetCommitment(providerId, hashedCommitmentId);

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
        public async Task<ActionResult> Acknowledgement(long providerId, string hashedCommitmentId, string message)
        {
            var commitment = (await _commitmentOrchestrator.GetCommitment(providerId, hashedCommitmentId));

            return View(new AcknowledgementViewModel
            {
                CommitmentReference = commitment.Reference,
                EmployerName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Message = message
            });
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


        [HttpGet]
        [Route("Cohorts")]
        public async Task<ActionResult> Cohorts(long providerId)
        {
            var model = await _commitmentOrchestrator.GetCohorts(providerId);
            return View(model);
        }

    }
}
