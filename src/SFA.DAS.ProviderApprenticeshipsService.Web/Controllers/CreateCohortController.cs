using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.Authorization.ProviderPermissions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ProviderUkPrnCheck]
    [RoutePrefix("{providerId}/apprentices")]
    public class CreateCohortController : BaseController
    {
        private readonly CreateCohortOrchestrator _createCohortOrchestrator;
        private readonly SelectEmployerOrchestrator _selectEmployerOrchestrator;

        public CreateCohortController(ICookieStorageService<FlashMessageViewModel> flashMessage,
            SelectEmployerOrchestrator selectEmployerOrchestrator, CreateCohortOrchestrator createCohortOrchestrator) : base(flashMessage)
        {
            _selectEmployerOrchestrator = selectEmployerOrchestrator;
            _createCohortOrchestrator = createCohortOrchestrator;
        }


        [HttpGet]
        [Route("cohorts/create")]
        public async Task<ActionResult> Create(long providerId)
        {
            var model = await _selectEmployerOrchestrator.GetChooseEmployerViewModel(providerId, EmployerSelectionAction.CreateCohort);

            return View("ChooseEmployer", model);
        }

        [HttpGet]
        [Route("cohorts/create/confirm-employer")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        public ActionResult ConfirmEmployer(long providerId, ConfirmEmployerViewModel confirmViewModel)
        {
            ModelState.Clear();
            if (!confirmViewModel.IsComplete)
            {
                return RedirectToAction("Create");
            }

            return View("ConfirmEmployer", confirmViewModel);
        }

        [HttpPost]
        [Route("cohorts/create/confirm-employer")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        public async Task<ActionResult> ConfirmEmployer(int providerId, ConfirmEmployerViewModel confirmViewModel)
        {
            if (confirmViewModel.Confirm.HasValue && !confirmViewModel.Confirm.Value)
            {
                return RedirectToAction("Create");
            }

            if (!ModelState.IsValid)
            {
                return View(confirmViewModel);
            }

            var hashedCommitmentId = await _createCohortOrchestrator.CreateCohort(providerId, confirmViewModel, CurrentUserId, GetSignedInUser());
            return RedirectToAction("Details", "Commitment", new { providerId, hashedCommitmentId });
        }
    }
}