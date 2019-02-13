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
using SFA.DAS.ProviderUrlHelper;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [ProviderUkPrnCheck]
    [RoutePrefix("{providerId}/apprentices")]
    public class ReservationController : BaseController
    {
        private readonly SelectEmployerOrchestrator _selectEmployerOrchestrator;
        private readonly ILinkGenerator _linkGenerator;

        public ReservationController(ICookieStorageService<FlashMessageViewModel> flashMessage,
            SelectEmployerOrchestrator selectEmployerOrchestrator, ILinkGenerator linkGenerator) : base(flashMessage)
        {
            _selectEmployerOrchestrator = selectEmployerOrchestrator;
            _linkGenerator = linkGenerator;
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

            var account = confirmViewModel.EmployerAccountPublicHashedId;
            var legalEntity = confirmViewModel.EmployerAccountLegalEntityPublicHashedId;
            return Redirect(_linkGenerator.ReservationsLink($"{providerId}/account/{account}/legalentity/{legalEntity}"));
        }
    }
}