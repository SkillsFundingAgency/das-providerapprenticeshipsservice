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
    public class ReservationController : BaseController
    {
        private readonly SelectEmployerOrchestrator _selectEmployerOrchestrator;

        public ReservationController(ICookieStorageService<FlashMessageViewModel> flashMessage,
            SelectEmployerOrchestrator selectEmployerOrchestrator) : base(flashMessage)
        {
            _selectEmployerOrchestrator = selectEmployerOrchestrator;
        }

        [HttpGet]
        [Route("reservations/create")]
        public async Task<ActionResult> Create(long providerId)
        {
            var model = await _selectEmployerOrchestrator.GetChooseEmployerViewModel(providerId, EmployerSelectionAction.CreateReservation);

            return View("ChooseEmployer", model);
        }

        [HttpGet]
        [Route("reservations/create/confirm-employer")]
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
        [Route("reservations/create/confirm-employer")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        public ActionResult ConfirmEmployer(int providerId, ConfirmEmployerViewModel confirmViewModel)
        {
            if (confirmViewModel.Confirm.HasValue && !confirmViewModel.Confirm.Value)
            {
                return RedirectToAction("Create");
            }

            if (!ModelState.IsValid)
            {
                return View(confirmViewModel);
            }

            return RedirectToRoute("account-home");
        }
    }
}