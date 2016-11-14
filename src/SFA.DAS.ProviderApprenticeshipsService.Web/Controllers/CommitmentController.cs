using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
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
        [Route("{providerId}/Commitments")]
        public async Task<ActionResult> Index(long providerId)
        {
            var model = await _commitmentOrchestrator.GetAll(providerId);

            return View(model);
        }

        [HttpGet]
        [Route("{providerId}/Commitment/{commitmentId}")]
        public async Task<ActionResult> Details(long providerId, long commitmentId)
        {
            var model = await _commitmentOrchestrator.Get(providerId, commitmentId);

            return View(model);
        }

        [HttpGet]
        [Route("{providerId}/Commitment/{commitmentId}/Edit/{apprenticeshipId}")]
        public async Task<ActionResult> Edit(long providerId, long commitmentId, long apprenticeshipId)
        {
            var model = await _commitmentOrchestrator.GetApprenticeship(providerId, commitmentId, apprenticeshipId);
            ViewBag.ApprenticeshipProgrammes = model.ApprenticeshipProgrammes;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(ApprenticeshipViewModel apprenticeship)
        {
            await _commitmentOrchestrator.UpdateApprenticeship(apprenticeship);

            return RedirectToAction("Details", new {providerId = apprenticeship.ProviderId, commitmentId = apprenticeship.CommitmentId });
        }

        [HttpGet]
        [Route("{providerId}/commitment/{CommitmentId}/AddApprentice")]
        public async Task<ActionResult> Create(long providerId, long commitmentId)
        {
            var model = await _commitmentOrchestrator.GetApprenticeship(providerId, commitmentId);
            ViewBag.ApprenticeshipProgrammes = model.ApprenticeshipProgrammes;

            return View(model.Apprenticeship);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{providerId}/commitment/{CommitmentId}/AddApprentice")]
        public async Task<ActionResult> Create(ApprenticeshipViewModel apprenticeship)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return await RedisplayCreateApprenticeshipView(apprenticeship);
                }

                await _commitmentOrchestrator.CreateApprenticeship(apprenticeship);
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex);

                return await RedisplayCreateApprenticeshipView(apprenticeship);
            }

            return RedirectToAction("Details", new { providerId = apprenticeship.ProviderId, commitmentId = apprenticeship.CommitmentId });
        }

        [HttpGet]
        public ActionResult FinishEditing(long providerId)
        {
            ViewBag.ProviderId = providerId;

            return View();
        }

        [HttpPost]
        public ActionResult FinishedEditingChoice(long providerId)
        {
            return RedirectToAction("Submit", new { providerId = providerId });
        }

        [HttpGet]
        // public async Task<ActionResult> Submit(long providerId, long commitmentId)
        public ActionResult Submit(long providerId)
        {
            // TODO: LWA Need to pass in parameters from request.
            //var commitment = await _commitmentOrchestrator.Get(providerId, commitmentId);

            var model = new SubmitCommitmentViewModel
            {
                SubmitCommitmentModel = new SubmitCommitmentModel
                {
                    ProviderId = providerId,
                    //CommitmentId = commitmentId
                },
                //Commitment = commitment.Commitment
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(SubmitCommitmentModel model)
        {
            //await _commitmentOrchestrator.SubmitApprenticeship(model);

            return RedirectToAction("Acknowledgement", new { providerId = model.ProviderId });
        }

        [HttpGet]
        public ActionResult Acknowledgement(long providerId)
        {
            ViewBag.ProviderId = providerId;

            return View();
        }

        private void AddErrorsToModelState(InvalidRequestException ex)
        {
            foreach (var error in ex.ErrorMessages)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        private async Task<ActionResult> RedisplayCreateApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var model = await _commitmentOrchestrator.GetApprenticeship(apprenticeship.ProviderId, apprenticeship.CommitmentId);
            model.Apprenticeship = apprenticeship;
            ViewBag.ApprenticeshipProducts = model.ApprenticeshipProgrammes;

            return View(model.Apprenticeship);
        }
    }
}