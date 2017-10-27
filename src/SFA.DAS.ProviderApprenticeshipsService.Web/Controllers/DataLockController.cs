using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ProviderUkPrnCheck]
    [RoutePrefix("{providerId}/apprentices/manage/{hashedApprenticeshipId}/datalock")]
    public class DataLockController : BaseController
    {
        private readonly DataLockOrchestrator _orchestrator;

        public DataLockController(
            ManageApprenticesOrchestrator orchestrator2,
            DataLockOrchestrator orchestrator,
            ICookieStorageService<FlashMessageViewModel> flashMessage) : base(flashMessage)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("", Name = "UpdateDataLock")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> UpdateDataLock(long providerId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetApprenticeshipMismatchDataLock(providerId, hashedApprenticeshipId);
            return View("UpdateDataLock", model);
        }

        [HttpPost]
        [Route("")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateDataLock(DataLockMismatchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = await _orchestrator.GetApprenticeshipMismatchDataLock(model.ProviderId, model.HashedApprenticeshipId);
                return View("UpdateDataLock", viewModel);
            }

            if (model.SubmitStatusViewModel == SubmitStatusViewModel.UpdateDataInIlr)
            {
                await _orchestrator.TriageMultiplePriceDataLocks(model.ProviderId, model.HashedApprenticeshipId, CurrentUserId, TriageStatus.FixIlr);

                return RedirectToAction("Details", "ManageApprentices", new { model.ProviderId, model.HashedApprenticeshipId });
            }

            if (model.SubmitStatusViewModel == SubmitStatusViewModel.Confirm)
            {
                return RedirectToAction("ConfirmDataLockChanges", new { model.ProviderId, model.HashedApprenticeshipId });
            }

            return RedirectToAction("Details", "ManageApprentices", new { model.ProviderId, model.HashedApprenticeshipId });
        }

        [HttpGet]
        [Route("confirm", Name = "UpdateDataLockConfirm")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ConfirmDataLockChanges(long providerId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetApprenticeshipMismatchDataLock(providerId, hashedApprenticeshipId);
            return View(model);
        }

        [HttpPost]
        [Route("datalock/confirm")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ConfirmDataLockChangesPost(DataLockMismatchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = await _orchestrator.GetApprenticeshipMismatchDataLock(model.ProviderId, model.HashedApprenticeshipId);
                return View("ConfirmDataLockChanges", viewModel);
            }

            if (model.SubmitStatusViewModel != null && model.SubmitStatusViewModel.Value == SubmitStatusViewModel.Confirm)
            {
                await _orchestrator.TriageMultiplePriceDataLocks(model.ProviderId, model.HashedApprenticeshipId, CurrentUserId, TriageStatus.Change);
            }

            return RedirectToAction("Details", "ManageApprentices", new { model.ProviderId, model.HashedApprenticeshipId });
        }

        [HttpGet]
        [Route("requestrestart", Name = "RequestRestart")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> RequestRestart(long providerId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetApprenticeshipMismatchDataLock(providerId, hashedApprenticeshipId);
            return View("RequestRestart", model);
        }

        [HttpPost]
        [Route("requestrestart")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RequestRestart(DataLockMismatchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var newModel = await _orchestrator.GetApprenticeshipMismatchDataLock(model.ProviderId, model.HashedApprenticeshipId);
                return View("RequestRestart", newModel);
            }

            if (model.SubmitStatusViewModel.HasValue && model.SubmitStatusViewModel.Value == SubmitStatusViewModel.Confirm)
            {
                return RedirectToAction(
                    "ConfirmRestart",
                    new { model.ProviderId, model.HashedApprenticeshipId });
            }

            if (model.SubmitStatusViewModel.HasValue
                && model.SubmitStatusViewModel.Value == SubmitStatusViewModel.UpdateDataInIlr)
            {
                var dataLock = model.DataLockSummaryViewModel.DataLockWithCourseMismatch.OrderBy(x => x.IlrEffectiveFromDate).First();
                await _orchestrator.UpdateDataLock(model.ProviderId, dataLock.DataLockEventId, model.HashedApprenticeshipId, SubmitStatusViewModel.UpdateDataInIlr, CurrentUserId);
                return RedirectToAction("Details", "ManageApprentices", new { model.ProviderId, model.HashedApprenticeshipId });
            }

            return RedirectToAction("Details", "ManageApprentices", new { model.ProviderId, model.HashedApprenticeshipId });
        }

        [HttpGet]
        [Route("ConfirmRestart", Name = "ConfirmRestart")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ConfirmRestart(long providerId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetConfirmRestartViewModel(providerId, hashedApprenticeshipId);
            return View("ConfirmRestart", model);
        }

        [HttpPost]
        [Route("ConfirmRestart")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ConfirmRestartPost(ConfirmRestartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var newModel = await _orchestrator.GetConfirmRestartViewModel(model.ProviderId, model.HashedApprenticeshipId);
                return View("ConfirmRestart", newModel);
            }
            if (model.SendRequestToEmployer.HasValue && model.SendRequestToEmployer.Value)
            {
                await _orchestrator.RequestRestart(model.ProviderId, model.DataLockEventId, model.HashedApprenticeshipId, CurrentUserId);
            }

            return RedirectToAction("Details", "ManageApprentices", new { model.ProviderId, model.HashedApprenticeshipId });
        }
    }
}