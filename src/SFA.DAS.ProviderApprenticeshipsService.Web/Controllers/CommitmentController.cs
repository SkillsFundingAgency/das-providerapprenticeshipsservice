using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

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
        [Route("Home")]
        public async Task<ActionResult> Index(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAll(providerId);

            return View(model);
        }

        [HttpGet]
        [Route("{commitmentId}/Details")]
        public async Task<ActionResult> Details(long providerId, long commitmentId)
        {
            var model = await _commitmentOrchestrator.GetCommitmentDetails(providerId, commitmentId);

            return View(model);
        }

        [HttpGet]
        [Route("{commitmentId}/Edit/{id}")]
        public async Task<ActionResult> Edit(long providerId, long commitmentId, long id)
        {
            var model = await _commitmentOrchestrator.GetApprenticeship(providerId, commitmentId, id);
            ViewBag.ApprenticeshipProgrammes = model.ApprenticeshipProgrammes;

            return View(model.Apprenticeship);
        }

        [HttpPost]
        [Route("{commitmentId}/Edit/{id}")]
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

            return RedirectToAction("Details", new {providerId = apprenticeship.ProviderId, commitmentId = apprenticeship.CommitmentId});
        }

        [HttpGet]
        [Route("{CommitmentId}/AddApprentice")]
        public async Task<ActionResult> Create(long providerId, long commitmentId)
        {
            var model = await _commitmentOrchestrator.GetCreateApprenticeshipViewModel(providerId, commitmentId);
            ViewBag.ApprenticeshipProgrammes = model.ApprenticeshipProgrammes;

            return View(model.Apprenticeship);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{CommitmentId}/AddApprentice")]
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

            return RedirectToAction("Details", new {providerId = apprenticeship.ProviderId, commitmentId = apprenticeship.CommitmentId});
        }

        [HttpGet]
        [Route("{commitmentId}/Finished")]
        public async Task<ActionResult> FinishEditing(long providerId, long commitmentId)
        {
            var viewModel = await _commitmentOrchestrator.GetFinishEditing(providerId, commitmentId);
            return View(viewModel);
        }

        [HttpPost]
        [Route("{commitmentId}/Finished")]
        public async Task<ActionResult> FinishEditing(FinishEditingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // TODO: Refactor out these magic strings
            if (!string.IsNullOrEmpty(viewModel.SaveOrSend) && viewModel.SaveOrSend.StartsWith("send"))
            {
                return RedirectToAction("Submit", 
                    new { providerId = viewModel.ProviderId, commitmentId = viewModel.CommitmentId, saveOrSend = viewModel.SaveOrSend});
            }

            if (viewModel.SaveOrSend == "approve")
            {
                await _commitmentOrchestrator.ApproveCommitment(viewModel.ProviderId, viewModel.CommitmentId, viewModel.SaveOrSend);
            }

            return RedirectToAction("Index", new {providerId = viewModel.ProviderId});
        }

        [HttpGet]
        public async Task<ActionResult> Submit(long providerId, long commitmentId, string saveOrSend)
        {
            var commitment = await _commitmentOrchestrator.GetCommitment(providerId, commitmentId);

            var model = new SubmitCommitmentViewModel
            {
                ProviderId = providerId,
                CommitmentId = commitmentId,
                EmployerName = commitment.LegalEntityName,
                SaveOrSend = saveOrSend
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(SubmitCommitmentViewModel model)
        {
            await _commitmentOrchestrator.SubmitCommitment(model);

            return RedirectToAction("Acknowledgement", new
            {
                providerId = model.ProviderId,
                commitmentId = model.CommitmentId,
                message = model.Message
            });
        }

        [HttpGet]
        public async Task<ActionResult> Acknowledgement(long providerId, long commitmentId, string message)
        {
            var commitment = (await _commitmentOrchestrator.GetCommitment(providerId, commitmentId));

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
            var model = await _commitmentOrchestrator.GetCreateApprenticeshipViewModel(apprenticeship.ProviderId, apprenticeship.CommitmentId);
            model.Apprenticeship = apprenticeship;
            ViewBag.ApprenticeshipProgrammes = model.ApprenticeshipProgrammes;

            return View(model.Apprenticeship);
        }
    }
}
